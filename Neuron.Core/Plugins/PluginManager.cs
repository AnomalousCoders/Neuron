using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Logging.Diagnostics;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Ninject;

namespace Neuron.Core.Plugins;

public class PluginManager
{
    private NeuronBase _neuronBase;
    private IKernel _kernel;
    private EventManager _eventManager;
    private MetaManager _metaManager;
    private NeuronLogger _neuronLogger;
    private ILogger _logger;

    public List<PluginContext> Plugins { get; }

    public readonly EventReactor<PluginLoadEvent> PluginLoad = new();
    public readonly EventReactor<PluginLoadEvent> PluginLoadLate = new();
    public readonly EventReactor<PluginUnloadEvent> PluginUnload = new();

    public PluginManager(NeuronBase neuronBase, IKernel kernel, MetaManager metaManager, EventManager eventManager, NeuronLogger neuronLogger)
    {
        _neuronBase = neuronBase;
        _kernel = kernel;
        _metaManager = metaManager;
        _neuronLogger = neuronLogger;
        _logger = neuronLogger.GetLogger<PluginManager>();
        _eventManager = eventManager;
        
        _eventManager.RegisterEvent(PluginLoad);
        _eventManager.RegisterEvent(PluginUnload);

        Plugins = new List<PluginContext>();
    }

    public void UnloadAll() => Plugins.ToList().ForEach(UnloadPlugin);
    
    public PluginContext LoadPlugin(IEnumerable<Type> types, Assembly assembly)
    {
        var batch = _metaManager.Analyze(types);
        var pluginAttributes = batch.Types.Where(x => x.TryGetAttribute<PluginAttribute>(out _)).Select(meta =>
        {
            meta.TryGetAttribute<PluginAttribute>(out var attribute);
            return (attribute, meta);
        }).ToArray();

        if (pluginAttributes.Length != 1) throw new Exception($"Expected single plugin but got {pluginAttributes.Length}");
        var first = pluginAttributes.First();

        var instance = (Plugin) first.meta.New();
        instance.NeuronLoggerInjected = _neuronLogger;
        _kernel.Bind(first.meta.Type).ToConstant(instance).InSingletonScope();
        
        var context = new PluginContext()
        {
            Plugin = instance,
            PluginType = first.meta.Type,
            Attribute = first.attribute,
            Batch = batch,
            Assembly = assembly
        };
        context.Lifecycle = new PluginLifecycle(context, _logger);
        Plugins.Add(context);

        try
        {
            instance.Load();
        }
        catch (Exception e) // Output exception as framework error
        {
            #region Framework Error
            var error = DiagnosticsError.FromParts(
                DiagnosticsError.Summary("An error occured while executing a plugin load"),
                DiagnosticsError.Description($"Invoking the Load() method of the plugin {context.Attribute.Name} " +
                                             $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}.")
            );
            error.Exception = e;
            NeuronDiagnosticHinter.AddCommonHints(e, error);
            _logger.Framework(error);
            throw;
            #endregion
        }
        
        var bindings = batch.GenerateBindings();
        context.MetaBindings = bindings;
        
        try {
            var loadEvent = new PluginLoadEvent { Context = context };
            PluginLoad.Raise(loadEvent);
            PluginLoadLate.Raise(loadEvent);
        }
        catch (Exception e) // Output exception as framework error
        {
            #region Framework Error
            var error = DiagnosticsError.FromParts(
                DiagnosticsError.Summary("An error occured while executing a plugin load event"),
                DiagnosticsError.Description($"Invoking the PluginLoad event for the plugin {context.Attribute.Name} " +
                                             $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}.")
            );
            error.Exception = e;
            NeuronDiagnosticHinter.AddCommonHints(e, error);
            _logger.Framework(error);
            throw;
            #endregion
        }
        
        _kernel.Inject(instance);
        
        context.Lifecycle.Enable.SubscribeAction(instance, context.PluginType.GetMethod(nameof(Plugin.Enable)));
        context.Lifecycle.Disable.SubscribeAction(instance, context.PluginType.GetMethod(nameof(Plugin.Disable)));

        context.Lifecycle.EnableSignal();
        
        return context;
    }

    public void ReloadPlugin(PluginContext context)
    {
        // Closest I can get to a reload with the current system
        context.Lifecycle.DisableSignal();
        context.Lifecycle.EnableSignal();
    }

    public void UnloadPlugin(PluginContext context)
    {
        Plugins.Remove(context);
        _kernel.Unbind(context.PluginType);
        context.Lifecycle.DisableSignal();
        
        var unloadEvent = new PluginUnloadEvent()
        {
            Context = context
        };
        
        try {
            PluginUnload.Raise(unloadEvent);
        }
        catch (Exception e) // Output exception as framework error
        {
            #region Framework Error
            var error = DiagnosticsError.FromParts(
                DiagnosticsError.Summary("An error occured while executing a plugin unload event"),
                DiagnosticsError.Description($"Invoking the PluginUnload event for the plugin {context.Attribute.Name} " +
                                             $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}.")
            );
            error.Exception = e;
            NeuronDiagnosticHinter.AddCommonHints(e, error);
            _logger.Framework(error);
            throw;
            #endregion
        }
    }
}

public class PluginLoadEvent : IEvent
{
    public PluginContext Context { get; set; }
}

public class PluginUnloadEvent : IEvent
{
    public PluginContext Context { get; set; }
}