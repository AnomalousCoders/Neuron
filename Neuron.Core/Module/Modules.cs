using System;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Ninject;
using Serilog;

namespace Neuron.Core.Module
{
    public class ModuleEvents
    {
        public EventReactor<VoidEvent> Enable = new();
        public EventReactor<VoidEvent> LateEnable = new();
        public EventReactor<VoidEvent> Disable = new();
    }

    public class ModuleLoadContext
    {
        public MetaBatchReference Batch { get; set; }
        public ModuleAttribute Attribute { get; set; }
        public Type[] Dependencies { get; set; }
        public Type ModuleType { get; set; }
        public Module Module { get; set; }
        public ModuleEvents Events { get; set; }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : MetaAttributeBase
    {
        public string Name { get; set; } = "Unnamed";
        public string Description { get; set; } = "";
        public Type[] Dependencies { get; set; } = Type.EmptyTypes;

    }
    
    public abstract class Module
    {
        [Inject]
        public NeuronLogger NeuronLoggerInjected { get; set; }

        protected ILogger Logger => NeuronLoggerInjected.GetLogger(GetType());
        
        public virtual void Load() { }
        public virtual void Enable() { }
        public virtual void LateEnable() { }
        public virtual void Disable() { }
    }
}