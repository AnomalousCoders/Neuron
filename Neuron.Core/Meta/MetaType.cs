using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neuron.Core.Meta;

public class MetaType
{
    public Type Type { get; set; }
    public MetaAttributeBase[] Attributes { get; set; }
    public MetaMethod[] Methods { get; set; }
    public MetaProperty[] Properties { get; set; }

    public bool TryGetAttribute<T>(out T output)
    {
        output = default;
        var matching = Attributes.OfType<T>().ToArray();
        if (matching.Length == 0) return false;
        output = matching[0];
        return true;
    }

    public bool Is<T>() => typeof(T).IsAssignableFrom(Type);

    public object New() => Activator.CreateInstance(Type);

    protected bool Equals(MetaType other)
    {
        return Equals(Type, other.Type);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((MetaType) obj);
    }

    public override int GetHashCode()
    {
        return (Type != null ? Type.GetHashCode() : 0);
    }

    public static MetaType Analyze(Type type)
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
}

public class MetaMethod
{
    public MethodInfo Method { get; set; }
    public MetaAttributeBase[] Attributes { get; set; }
}

public class MetaProperty
{
    public PropertyInfo Property { get; set; }
    public MetaAttributeBase[] Attributes { get; set; }
}