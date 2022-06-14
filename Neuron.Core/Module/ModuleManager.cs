using System;
using System.Collections.Generic;
using System.Linq;
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

    public void LoadModule(IEnumerable<Type> types)
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
            Dependencies = first!.attribute.Dependencies,
            ModuleType = instance.GetType(),
            Events = new ModuleEvents()
        };
            
        _moduleBuffer.Add(context);
    }

    public void ActivateModules()
    {
        IsLocked = true;
        var solved = SolveDependencies();
        while (solved.Count != 0)
        {
            var context = solved.Dequeue();
            var batchResolved = context.Batch.Process();
            var services = batchResolved.OfType<Service>();

            context.Module = (Module)_kernel.Get(context.ModuleType);
            _kernel.BindSimple(context.Module);
            context.Module.Load();
                
            foreach (var service in services)
            {
                context.Events.Enable.SubscribeAction(service, service.GetType().GetMethod(nameof(Service.Enable)));
                context.Events.Disable.SubscribeAction(service, service.GetType().GetMethod(nameof(Service.Disable)));
                context.Events.Disable.Subscribe(_ => _serviceManager.UnbindService(service));
            }

            context.Events.Enable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.Enable)));
            context.Events.LateEnable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.LateEnable)));
            context.Events.Disable.SubscribeAction(context.Module, context.ModuleType.GetMethod(nameof(Module.Disable)));
            context.Events.Disable.Subscribe(_ => _kernel.Unbind(context.ModuleType));

            _activeModules.Add(context);
        }
    }

    public void EnableAll()
    {
        foreach (var module in _activeModules)
        {
            module.Events.Enable.Raise();
        }
        foreach (var module in _activeModules)
        {
            module.Events.LateEnable.Raise();
        }
    }
        
    public void DisableAll()
    {
        foreach (var module in _activeModules)
        {
            module.Events.Disable.Raise();
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
                if (context.Dependencies.Length == 0)
                {
                    resolved.Add(context);
                    continue;
                }

                if (context.Dependencies.All(dep => loadingQueue.Any(loaded => loaded.ModuleType == dep)))
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