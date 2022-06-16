using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neuron.Core.Events;
using Neuron.Core.Logging;

namespace Neuron.Core.Meta;

/// <summary>
/// Neuron service for reflection based meta analysis of types.
/// </summary>
public class MetaManager
{

    public EventReactor<MetaLoadedEvent> MetaLoad = new();
    public EventReactor<MetaGenerateBindingsEvent> MetaGenerateBindings = new();
    public readonly HashSet<MetaType> MetaTypes = new();
    
    private ILogger _logger;

    public MetaManager(NeuronLogger neuronLogger)
    {
        _logger = neuronLogger.GetLogger<MetaManager>();
    }
    
    /// <summary>
    /// Removes the specified types from the meta types set,
    /// making them no longer resolvable via <see cref="Resolve"/>.
    /// </summary>
    public void Untrack(List<MetaType> list)
    {
        foreach (var type in list)
        {
            MetaTypes.Remove(type);
        }
    }
    
    /// <summary>
    /// Resolves the specified type using the <see cref="MetaTypes"/> set.
    /// </summary>
    /// <returns>Correlated MetaType or null if not present</returns>
    public MetaType Resolve(Type type) => MetaTypes.FirstOrDefault(x => x.Type == type);

    /// <summary>
    /// Generates bindings for specified types using the <see cref="MetaGenerateBindings"/> event.
    /// Bindings are intermediate data-holders which are later used by the framework or modules,
    /// and are publicly available if related with an module. Can be retrieved via <see cref="Modules.ModuleManager"/>
    /// as a property of <see cref="Modules.ModuleLoadContext"/>.
    /// </summary>
    public List<object> GenerateBindings(List<MetaType> types)
    {
        var list = new List<object>();
        foreach (var type in types)
        {
            MetaGenerateBindings.Raise(new MetaGenerateBindingsEvent
            {
                MetaType = type,
                Outputs = list
            });
            _logger.Debug("* [FullName] has been processed", type.Type.FullName);
        }

        return list;
    }
  
    /// <summary>
    /// Performs <see cref="MetaType.TryGetMetaType"/> on all types of the assembly and returns
    /// the resulting MetaTypes in an <see cref="MetaBatchReference"/> wrapper.
    /// </summary>
    public MetaBatchReference Analyze(Assembly assembly) => Analyze(assembly.GetTypes());
    
    /// <summary>
    /// Performs <see cref="MetaType.TryGetMetaType"/> on the specified types and returns
    /// the resulting MetaTypes in an <see cref="MetaBatchReference"/> wrapper.
    /// </summary>
    public MetaBatchReference Analyze(IEnumerable<Type> types)
    {
        var processed = AnalyzeGroup(types);
        var addedList = new List<MetaType>();
        foreach (var type in processed)
        {
            var added = MetaTypes.Add(type);
            if (!added) continue;
            addedList.Add(type);
            MetaLoad.Raise(new MetaLoadedEvent
            {
                Type = type 
            });
            _logger.Debug("* [FullName] has been meta analyzed", type.Type.FullName);
        }

        return new MetaBatchReference
        {
            Types = addedList,
            Reference = this
        };
    }

    private static List<MetaType> AnalyzeGroup(IEnumerable<Type> types) => types
        .Select(MetaType.TryGetMetaType)
        .Where(selected => selected != null)
        .ToList();
}

public class MetaLoadedEvent : IEvent
{
    public MetaType Type { get; internal set; }
}

public class MetaGenerateBindingsEvent : IEvent
{
    public MetaType MetaType { get; internal set; }
    public List<object> Outputs { get; internal set; }
}