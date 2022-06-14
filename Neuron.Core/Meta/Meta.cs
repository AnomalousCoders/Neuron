using System;
using System.Diagnostics;
using System.Reflection;

namespace Neuron.Core.Meta
{
    public abstract class MetaAttributeBase : Attribute {}

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Method)]
    public class MetaAttribute : MetaAttributeBase {}
    
    [Meta]
    public interface IMetaObject {}
}