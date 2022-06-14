using System;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Ninject;
using Serilog;

namespace Neuron.Core.Module;

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

[AttributeUsage(AttributeTargets.Class)]
public class ModuleAttribute : MetaAttributeBase
{
    public string Name { get; set; } = "Unnamed";
    public string Description { get; set; } = "";
    public Type[] Dependencies { get; set; } = Type.EmptyTypes;

}