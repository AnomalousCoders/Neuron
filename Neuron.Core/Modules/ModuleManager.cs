using System;
using System.Collections.Generic;
using System.Linq;
using Neuron.Core.Dependencies;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Logging.Diagnostics;
using Neuron.Core.Logging.Utils;
using Neuron.Core.Meta;
using Ninject;

namespace Neuron.Core.Modules;

public class ModuleManager
{
    private NeuronBase _neuronBase;
    private IKernel _kernel;
    private MetaManager _metaManager;
    private ServiceManager _serviceManager;
    private NeuronLogger _neuronLogger;
    private ILogger _logger;

    private List<ModuleLoadContext> _moduleBuffer;
    private List<ModuleLoadContext> _activeModules;

    public bool IsLocked { get; private set; } = false;

    private readonly EventReactor<ModuleLoadEvent> ModuleLoad = new();

    public ModuleManager(NeuronBase neuronBase, MetaManager metaManager, NeuronLogger neuronLogger, IKernel kernel, ServiceManager serviceManager)
    {
        _neuronBase = neuronBase;
        _metaManager = metaManager;
        _neuronLogger = neuronLogger;
        _kernel = kernel;
        _serviceManager = serviceManager;
        _moduleBuffer = new List<ModuleLoadContext>();
        _activeModules = new List<ModuleLoadContext>();
        _logger = _neuronLogger.GetLogger<ModuleManager>();
    }
    
    public bool HasModule(string name)
        => _activeModules.Any(x => String.Equals(name, x.Attribute.Name, StringComparison.OrdinalIgnoreCase));

    public ModuleLoadContext Get(string name) 
        => _activeModules.FirstOrDefault(x => String.Equals(name, x.Attribute.Name, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<ModuleLoadContext> GetAllModules() => _activeModules;

    public ModuleLoadContext LoadModule(IEnumerable<Type> types)
    {
        var batch = _metaManager.Analyze(types);
        var moduleAttributes = batch.Types.Where(x => x.TryGetAttribute<ModuleAttribute>(out _)).Select(meta =>
        {
            meta.TryGetAttribute<ModuleAttribute>(out var attribute);
            return (attribute, meta);
        }).ToArray();

        if (moduleAttributes.Length != 1) throw new Exception($"Expected single module but got {moduleAttributes.Length}");
            
        var first = moduleAttributes.FirstOrDefault();
        var instance = first.meta.New();

        var context = new ModuleLoadContext()
        {
            Attribute = first.attribute,
            Batch = batch,
            ModuleDependencies = first!.attribute.Dependencies,
            ModuleType = instance.GetType()
        };
        context.Lifecycle = new ModuleLifecycle(context, _logger);
            
        _moduleBuffer.Add(context);
        return context;
    }

    public void ActivateModules()
    {
        IsLocked = true;
        
        var moduleResolver = new CyclicDependencyResolver<ModuleLoadContext>();
        moduleResolver.AddDependables(_activeModules);
        moduleResolver.AddDependencies(_moduleBuffer);
        var moduleResult = moduleResolver.Resolve();

        _logger.Debug("[Header] Dependency Tree\n[Tree]","Modules", moduleResolver.BuildTree(moduleResult));
        
        if (!moduleResult.Successful)
        {
            foreach (var context in moduleResult.Unsolved)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("Unsatisfied module dependencies"),
                    DiagnosticsError.Description($"Could not resolve all module dependencies for module '{context.Attribute.Name}'.")
                );

                var missing = context.Dependencies
                    .Where(x => !moduleResult.Dependables.Contains(x));
                error.Nodes.Add(DiagnosticsError.Description($"The module is missing following dependencies: {string.Join(", ", missing)}"));
                
                error.Nodes.Add(DiagnosticsError.Property("Module Dependencies", "Tree View\n" + moduleResolver.BuildTree(moduleResult)));
                _logger.Framework(error);
            }
        }
        
        foreach (var context in moduleResult.Solved)
        {
            var batchGeneratedBindings = context.Batch.GenerateBindings();
            var emittedServices = batchGeneratedBindings.OfType<ServiceRegistration>();

            context.MetaBindings = batchGeneratedBindings; // Make bindings permanently accessible
            context.Module = (Module)Activator.CreateInstance(context.ModuleType); // Create un-injected module instance
            context.Module.NeuronLoggerInjected = _neuronLogger; // Manually inject logger
            _kernel.Bind(context.ModuleType).ToConstant(context.Module).InSingletonScope(); // Make module available for kernel without injecting
            
            try
            {
                context.Module.Load();
            }
            catch (Exception e) // Output exception as framework error
            {
                #region Framework Error
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while executing a module load"),
                    DiagnosticsError.Description($"Invoking the Load() method of the module {context.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}.")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                _logger.Framework(error);
                throw;
                #endregion
            }

            #region Resolve Services and Service Dependencies
            var serviceResolver = new CyclicDependencyResolver<ServiceRegistration>();
            foreach (var service in emittedServices)
            {
                serviceResolver.AddDependency(service);
                foreach (var dependency in service.Dependencies)
                {
                    if (dependency is not Type type) continue;
                    if (_kernel.GetBindings(type).Any()) serviceResolver.AddDependable(type);
                }
            }

            var serviceResult = serviceResolver.Resolve();
            if (serviceResult.Successful)
            {
                _logger.Debug("[Module] Services:\n[Tree]", LogBox.Of(context.ModuleType.Name),
                    LogBox.Of(serviceResolver.BuildTree(serviceResult)));
            }
            else  // Output dependency problem as framework error
            {
                #region Framework Error
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("Unsatisfied service dependencies"),
                    DiagnosticsError.Description($"Could not resolve all dependencies for services of module '{context.Attribute.Name}'.")
                );
                foreach (var registration in serviceResult.Unsolved)
                {
                    var missing = registration.Dependencies.Where(x => !serviceResult.Dependables.Contains(x))
                        .Select(x => x.ToString()).ToList();
                    error.Nodes.Add(DiagnosticsError.Description($"Service {registration.MetaType.Type.FullName} is " +
                                                                 $"missing following bindings: {string.Join(", ", missing)}"));
                }
                
                error.Nodes.Add(DiagnosticsError.Property("Service Dependencies", "Tree View\n" + serviceResolver.BuildTree(serviceResult)));
                _logger.Framework(error);
                #endregion
            }
            #endregion

            #region Resolve Module Dependencies
            var modulePropertyResolver = new CyclicDependencyResolver<ModulePropertyDependencyHolder>();
            var modulePropertyDeps = new List<object>();
            foreach (var service in serviceResult.Solved)
            {
                modulePropertyResolver.AddDependable(service.ServiceType);
            }
            foreach (var type in KernelDependencyResolver.GetPropertyDependencies(context.ModuleType))
            {
                modulePropertyDeps.Add(type);
                if (_kernel.GetBindings(type).Any()) modulePropertyResolver.AddDependable(type);
            }
            #endregion

            #region Resolve Module Property Dependencies
            var moduleDep = new ModulePropertyDependencyHolder(modulePropertyDeps);
            modulePropertyResolver.AddDependency(moduleDep);
            var modulePropertyResult = modulePropertyResolver.Resolve();
            if (!modulePropertyResult.Successful) // Output module dependency problem as framework error
            {
                #region Framework Error
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("Unsatisfied module property dependencies"),
                    DiagnosticsError.Description($"Could not resolve all property dependencies for module '{context.Attribute.Name}'.")
                );
                
                var missing = moduleDep.Dependencies.Where(x => !modulePropertyResult.Dependables.Contains(x))
                    .Select(x => x.ToString()).ToList();
                error.Nodes.Add(DiagnosticsError.Description($"The module is missing following bindings: {string.Join(", ", missing)}"));
                
                error.Nodes.Add(DiagnosticsError.Property("Module Dependencies", "Tree View\n" + modulePropertyResolver.BuildTree(modulePropertyResult)));
                _logger.Framework(error);
                
                var modHanGra = _neuronBase.Configuration.Engine.GracefulMissingServiceDependencies;
                if (!modHanGra) continue;
                #endregion
            }
            #endregion

            var srvHanGra = _neuronBase.Configuration.Engine.GracefulMissingServiceDependencies;
            foreach (var registration in (srvHanGra ? serviceResult.Dependencies : serviceResult.Solved))
            {
                _serviceManager.BindService(registration);
                var serv = _kernel.Get(registration.ServiceType); // Create service using kernel
                
                // Hook service lifecycle to module lifecycle
                context.Lifecycle.EnableComponents.SubscribeAction(serv,
                    registration.MetaType.Type.GetMethod(nameof(Service.Enable)));
                context.Lifecycle.DisableComponents.SubscribeAction(serv,
                    registration.MetaType.Type.GetMethod(nameof(Service.Disable)));
                context.Lifecycle.DisableComponents.Subscribe(_ => _serviceManager.UnbindService(registration));
            }
            
            try {
                var loadEvent = new ModuleLoadEvent { Context = context };
                ModuleLoad.Raise(loadEvent);
            }
            catch (Exception e) // Output exception as framework error
            {
                #region Framework Error
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while executing a module load event"),
                    DiagnosticsError.Description($"Invoking the ModuleLoad event for the module {context.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}.")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                _logger.Framework(error);
                throw;
                #endregion
            }
            
            context.Lifecycle.Enable.Subscribe(delegate
            {
                // Dependency checks at this point make no sense anymore since Ninject keeps messing with the bindings,
                // making it almost impossible to check if there is a correct binding which hasn't been created as a
                // side object through some service. The probability of an unknown dependency problem at this stage
                // is also very unlikely since bindings are also validated before service construction.
                _kernel.Inject(context.Module);
            });

            // Hook module class lifecycle to module lifecycle
            context.Lifecycle.Enable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.Enable)));
            context.Lifecycle.LateEnable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.LateEnable)));
            context.Lifecycle.Disable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.Disable)));
            context.Lifecycle.Disable.Subscribe(_ => _kernel.Unbind(context.ModuleType));

            // Save context reference
            _activeModules.Add(context);
        }
    }

    public void EnableAll()
    {
        foreach (var module in _activeModules)
        {
            module.Lifecycle.EnableSignal();
        }
        foreach (var module in _activeModules)
        {
            try
            {
                module.Lifecycle.LateEnable.Raise();
            }
            catch (Exception e)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while late enabling a module"),
                    DiagnosticsError.Description($"Invoking the Module Late Enable Events of the module {module.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}."),
                    DiagnosticsError.Hint("This exception most commonly occurs when a module throws an exception in its LateEnable() method")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                _logger.Framework(error);
            }
        }
    }
        
    public void DisableAll()
    {
        foreach (var module in _activeModules)
        {
            module.Lifecycle.DisableSignal();
        }
    }
}

internal class ModulePropertyDependencyHolder : SimpleDependencyHolderBase
{
    public override IEnumerable<object> Dependencies { get; }

    public ModulePropertyDependencyHolder(IEnumerable<object> dependencies)
    {
        Dependencies = dependencies;
    }

    public override object Dependable { get; } = new();
}

public class ModuleLoadEvent : IEvent
{
    public ModuleLoadContext Context { get; set; }
}