using System;
using System.Collections.Generic;
using System.Linq;
using Neuron.Core.Dependencies;
using Neuron.Core.Events;
using Neuron.Core.Logging;
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
    }

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

        var logger = _neuronLogger.GetLogger<ModuleManager>();
        logger.Debug("[Header] Dependency Tree\n[Tree]",LogBox.Of("Modules"), LogBox.Of(moduleResolver.BuildTree(moduleResult)));
        
        if (!moduleResult.Successful)
        {
            logger.Warn("Module dependencies not resolved");
        }
        
        foreach (var context in moduleResult.Solved)
        {;
            var batchResolved = context.Batch.Process();
            var emittedServices = batchResolved.OfType<ServiceRegistration>();

            context.Module = (Module)Activator.CreateInstance(context.ModuleType);
            context.Module.NeuronLoggerInjected = _neuronLogger;
            _kernel.Bind(context.ModuleType).ToConstant(context.Module).InSingletonScope();
            context.Module.Load();

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
            if (serviceResult.Successful) logger.Debug("{Module} Services:\n{Tree}", LogBox.Of(context.ModuleType.Name),
                    LogBox.Of(serviceResolver.BuildTree(serviceResult)));
            else logger.Error("{Module} Services [ERROR]:\n{Tree}", LogBox.Of(context.ModuleType.Name),
                    LogBox.Of(serviceResolver.BuildTree(serviceResult)));

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
                foreach (var dependency in KernelDependencyResolver.GetPropertyDependencies(context.ModuleType))
                {
                    var isResolvable = _kernel.GetBindings(dependency).Any();
                    if (!isResolvable) throw new Exception($"Module {context.ModuleType} has unsolved dependency {dependency}");
                }
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
            module.Lifecycle.EnableSignal();
        }
        foreach (var module in _activeModules)
        {
            module.Lifecycle.LateEnable.Raise();
        }
    }
        
    public void DisableAll()
    {
        foreach (var module in _activeModules)
        {
            module.Lifecycle.DisableSignal();
        }
    }
        
    private Queue<ModuleLoadContext> SolveDependencies()
    {
        var remaining = _moduleBuffer.ToList();
        var loadingQueue = new Queue<ModuleLoadContext>();

        const int maxDepth = 255;
        var depth = 0;
        while (depth < maxDepth)
        {
            var resolved = new List<ModuleLoadContext>();
            foreach (var context in remaining)
            {
                if (context.ModuleDependencies.Length == 0)
                {
                    resolved.Add(context);
                    continue;
                }

                if (context.ModuleDependencies.All(dep => loadingQueue.Any(loaded => loaded.ModuleType == dep)))
                {
                    resolved.Add(context);
                }
            }
            foreach (var context in resolved)
            {
                remaining.Remove(context);
                loadingQueue.Enqueue(context);
            }

            if (resolved.Count == 0) break;
            depth++;
        }

        if (remaining.Count == 0) return loadingQueue;
            
        var unsatisfiedString = string.Join(", ", remaining.Select(x => x.ModuleType.Name));
        throw new Exception($"{unsatisfiedString} got unsatisfied dependencies!");
    }
}