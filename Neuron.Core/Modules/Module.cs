using System;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Ninject;

namespace Neuron.Core.Modules;

public abstract class Module : InjectedLoggerBase
{
    public virtual void Load(IKernel kernel) { }
    public virtual void Enable() { }
    public virtual void LateEnable() { }
    public virtual void Disable() { }
}

[AttributeUsage(AttributeTargets.Class)]
public class ModuleAttribute : MetaAttributeBase
{
    public string Name { get; set; } = "Unnamed Module";
    public string Description { get; set; } = "no description provided";
    public string Version { get; set; } = "1.0.0.0";
    
    public string Author { get; set; }
    public string Website { get; set; }
    public string Repository { get; set; }
    
    public Type[] Dependencies { get; set; } = Type.EmptyTypes;
}