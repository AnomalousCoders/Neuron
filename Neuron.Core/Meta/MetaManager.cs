using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neuron.Core.Events;
using Neuron.Core.Logging;

namespace Neuron.Core.Meta;

public class MetaManager
{

    public EventReactor<MetaLoadedEvent> MetaLoad = new();
    public EventReactor<MetaProcessEvent> MetaProcess = new();
    public readonly HashSet<MetaType> MetaTypes = new();
    
    private ILogger _logger;

    public MetaManager(NeuronLogger neuronLogger)
    {
        _logger = neuronLogger.GetLogger<MetaManager>();
    }
    

    public void Untrack(List<MetaType> list)
    {
        foreach (var type in list)
        {
            MetaTypes.Remove(type);
        }
    }
    
    public MetaType Resolve(Type type) 
        => MetaTypes.First(x => x.Type == type);

    public List<object> Process(List<MetaType> types)
    {
        var list = new List<object>();
        foreach (var type in types)
        {
            MetaProcess.Raise(new MetaProcessEvent
            {
                MetaType = type,
                Outputs = list
            });
            _logger.Debug("* [FullName] has been processed", type.Type.FullName);
        }

        return list;
    }
  
    public MetaBatchReference Analyze(Assembly assembly) => Analyze(assembly.GetTypes());
    
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

    private static List<MetaType> AnalyzeGroup(IEnumerable<Type> types) => types.Select(MetaType.ExclusiveAnalyze).Where(selected => selected != null).ToList();
}

public class MetaLoadedEvent : IEvent
{
    public MetaType Type { get; internal set; }
}

public class MetaProcessEvent : IEvent
{
    public MetaType MetaType { get; internal set; }
    public List<object> Outputs { get; internal set; }
}