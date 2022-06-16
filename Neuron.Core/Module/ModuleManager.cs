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

namespace Neuron.Core.Module;

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


    public bool HasModule(string name) => _activeModules.Any(x => string.Equals(name, x.Attribute.Name, StringComparison.OrdinalIgnoreCase));

    public ModuleLoadContext Get(string name) => _activeModules.FirstOrDefault(x => string.Equals(name, x.Attribute.Name, StringComparison.OrdinalIgnoreCase));

    public ModuleLoadContext LoadModule(IEnumerable<Type> types)
    {
        var batch = _metaManager.Analyze(types);
        var modules = batch.Types.Where(x => x.TryGetAttribute<ModuleAttribute>(out _)).Select(meta =>
        {
            meta.TryGetAttribute<ModuleAttribute>(out var attribute);
            return (attribute, meta);
        }).ToArray();

        if (modules.Length != 1) throw new Exception($"Expected single module but got {modules.Length}");
            
        var first = modules.FirstOrDefault();
        var instance = first.meta.New();

        var context = new ModuleLoadContext()
        {
            Attribute = first.attribute,
            Batch = batch,
            ModuleDependencies = first!.attribute.Dependencies,
            ModuleType = instance.GetType(),
            Lifecycle = new ModuleLifecycle()
        };
            
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
            var batchResolved = context.Batch.Process();
            var emittedServices = batchResolved.OfType<ServiceRegistration>();

            context.Module = (Module)Activator.CreateInstance(context.ModuleType);
            context.Module.NeuronLoggerInjected = _neuronLogger;
            _kernel.Bind(context.ModuleType).ToConstant(context.Module).InSingletonScope();
            try
            {
                context.Module.Load();
            }
            catch (Exception e)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while executing a module load"),
                    DiagnosticsError.Description($"Invoking the Load() method of the module {context.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}.")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                _logger.Framework(error);
                throw;
            }

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
            if (serviceResult.Successful) _logger.Debug("[Module] Services:\n[Tree]", LogBox.Of(context.ModuleType.Name),
                    LogBox.Of(serviceResolver.BuildTree(serviceResult)));
            else
            {
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
            }

            
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

            var moduleDep = new ModulePropertyDependencyHolder(modulePropertyDeps);
            modulePropertyResolver.AddDependency(moduleDep);
            var modulePropertyResult = modulePropertyResolver.Resolve();
            if (!modulePropertyResult.Successful)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("Unsatisfied module dependencies"),
                    DiagnosticsError.Description($"Could not resolve all property dependencies for module '{context.Attribute.Name}'.")
                );
                
                var missing = moduleDep.Dependencies.Where(x => !modulePropertyResult.Dependables.Contains(x))
                    .Select(x => x.ToString()).ToList();
                error.Nodes.Add(DiagnosticsError.Description($"The module is missing following bindings: {string.Join(", ", missing)}"));
                
                error.Nodes.Add(DiagnosticsError.Property("Module Dependencies", "Tree View\n" + modulePropertyResolver.BuildTree(modulePropertyResult)));
                _logger.Framework(error);
            }

            foreach (var registration in serviceResult.Solved)
            {
                if (_kernel.CheckDependencies(registration.MetaType.Type))
                {
                    _serviceManager.BindService(registration);
                    var serv = _kernel.Get(registration.ServiceType);
                    context.Lifecycle.EnableComponents.SubscribeAction(serv,
                        registration.MetaType.Type.GetMethod(nameof(Service.Enable)));
                    context.Lifecycle.DisableComponents.SubscribeAction(serv,
                        registration.MetaType.Type.GetMethod(nameof(Service.Disable)));
                    context.Lifecycle.DisableComponents.Subscribe(_ => _serviceManager.UnbindService(registration));
                }
                else throw new Exception("Unbound Kernel Dependency");
            }

            context.Lifecycle.Enable.Subscribe(delegate
            {
                // Dependency checks at this point make no sense anymore since Ninject keeps messing with the bindings,
                // making it almost impossible to check if there is a correct binding which hasn't been created as a
                // side object through some service. The probability of an unknown dependency problem at this stage
                // is also very unlikely since bindings are also validated before service construction.
                _kernel.Inject(context.Module);
            });
            
            context.Lifecycle.Enable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.Enable)));
            context.Lifecycle.LateEnable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.LateEnable)));
            context.Lifecycle.Disable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.Disable)));
            context.Lifecycle.Disable.Subscribe(_ => _kernel.Unbind(context.ModuleType));

            _activeModules.Add(context);
        }
    }

    public void EnableAll()
    {
        
        foreach (var module in _activeModules)
        {
            module.Lifecycle.EnableSignal(_logger, module);
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
            module.Lifecycle.DisableSignal(_logger, module);
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