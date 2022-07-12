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
    public string Name { get; set; } = "Unnamed Plugin";
    public string Description { get; set; } = "no description provided";
    
    public string Author { get; set; }
    public string Website { get; set; }
    public string Repository { get; set; }
    
    public string Version { get; set; } = "1.0.0.0";
}