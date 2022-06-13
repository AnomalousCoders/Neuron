using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Neuron.Core.Meta
{
    public abstract class MetaAttributeBase : Attribute {}

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Method)]
    public class MetaAttribute : MetaAttributeBase {}
    
    [Meta]
    public interface IMetaObject {}
    
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

}