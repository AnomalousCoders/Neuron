using System;
using Neuron.Core.Meta;

namespace Neuron.Core.Plugins;

public class Plugin : InjectedLoggerBase, IMetaObject
{
    public virtual void Load() { }
    public virtual void Enable() { }
    public virtual void Disable() { }
}

[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute : Attribute
{
    public string Name { get; set; } = "Unnamed";
    public string Description { get; set; } = "";
    public string Version { get; set; } = "1.0.0.0";
}