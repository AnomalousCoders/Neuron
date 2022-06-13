using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Ninject;
using Serilog;

namespace Neuron.Core.Meta;

public class MetaManager
{

    public EventReactor<MetaLoadedEvent> MetaLoad = new();
    public EventReactor<MetaProcessEvent> MetaProcess = new();

    private ILogger _logger;

    public MetaManager(NeuronLogger neuronLogger)
    {
        _logger = neuronLogger.GetLogger<MetaManager>();
    }


    public readonly HashSet<MetaType> MetaTypes = new();

    public void Untrack(List<MetaType> list)
    {
        foreach (var type in list)
        {
            MetaTypes.Remove(type);
        }
    }
    
    public MetaType Resolve(Type type) => MetaTypes.First(x => x.Type == type);
    
    public MetaBatchReference Analyze(IEnumerable<Type> types)
    {
        var processed = SingleAnalyse(types);
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
            _logger.Debug("* {FullName} has been meta analyzed", type.Type.FullName);
        }

        return new MetaBatchReference
        {
            Types = addedList,
            Reference = this
        };
    }

    public ArrayList Process(List<MetaType> types)
    {
        var list = new ArrayList();
        foreach (var type in types)
        {
            MetaProcess.Raise(new MetaProcessEvent
            {
                Type = type,
                Outputs = list
            });
            _logger.Debug("* {FullName} has been processed", type.Type.FullName);
        }

        return list;
    }
    
    public List<MetaType> SingleAnalyse(IEnumerable<Type> types) => types.Select(type => SingleAnalyse(type)).Where(selected => selected != null).ToList();

    public MetaType SingleAnalyse(Type type)
    {
        var keepType = false;
        var metaAttributesList = type.GetCustomAttributes(true)
            .OfType<MetaAttributeBase>().ToList();
        var interfaceAttributes = ReflectionUtils
            .ResolveInterfaceAttributes(type)
            .OfType<MetaAttributeBase>();
        metaAttributesList.AddRange(interfaceAttributes);
        var metaAttributes = metaAttributesList.ToArray();
        if (metaAttributes.Length != 0) keepType = true;

        var metaMethods = new List<MetaMethod>();
        var metaProperties = new List<MetaProperty>();
            
        foreach (var x in type.GetRuntimeMethods())
        {
            var methodMetaAttributes = x
                .GetCustomAttributes(true).OfType<MetaAttributeBase>().ToArray();

            if (methodMetaAttributes.Length != 0) keepType = true;
                
            metaMethods.Add(new MetaMethod
            {
                Method = x,
                Attributes = methodMetaAttributes.ToArray()
            });
        }
            
        foreach (var x in type.GetRuntimeProperties())
        {
            var propertyMetaAttributes = x
                .GetCustomAttributes(true).OfType<MetaAttributeBase>().ToArray();
                
            if (propertyMetaAttributes.Length != 0) keepType = true;
                
            metaProperties.Add(new MetaProperty
            {
                Property = x,
                Attributes = propertyMetaAttributes.ToArray()
            });
        }

        if (!keepType) return null;
        return new MetaType
        {
            Type = type,
            Attributes = metaAttributes,
            Methods = metaMethods.ToArray(),
            Properties = metaProperties.ToArray()
        };
    }

    public class MetaBatchReference
    {
        internal MetaManager Reference { private get; set; }
        public List<MetaType> Types { get; internal set; }

        public ArrayList Process() => Reference.Process(Types);

        public void Untrack() => Reference.Untrack(Types);
    }
}

public class MetaLoadedEvent : IEvent
{
    public MetaType Type { get; internal set; }
}

public class MetaProcessEvent : IEvent
{
    public MetaType Type { get; internal set; }
    public ArrayList Outputs { get; internal set; }
}