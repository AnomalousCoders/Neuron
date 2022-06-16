using System;

namespace Neuron.Core.Meta
{
    /// <summary>
    /// Base for more specific meta attributes.
    /// Marks an object as being relevant for neurons attribute based programming.
    /// </summary>
    public abstract class MetaAttributeBase : Attribute {}
    
    /// <summary>
    /// Marks an object as being relevant for neurons attribute based programming.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Method)]
    public class MetaAttribute : MetaAttributeBase {}
    
    /// <summary>
    /// Base for more specific meta objects.
    /// Marks an object as being relevant for neurons attribute based programming.
    /// </summary>
    [Meta]
    public interface IMetaObject {}
    
    /// <summary>
    /// Marks an object as universally automatic registrable.
    /// In the context of a command, this would mean automatically subscribing
    /// it to the command manager, for listeners this would mean automatically
    /// registering it at the event manager.
    /// </summary>
    public class AutomaticAttribute : MetaAttributeBase {}
}