using System;
using Neuron.Core.Meta;

namespace Neuron.Core.Module;

public class ModuleLoadContext
{
    public MetaBatchReference Batch { get; set; }
    public ModuleAttribute Attribute { get; set; }
    public Type[] Dependencies { get; set; }
    public Type ModuleType { get; set; }
    public Module Module { get; set; }
    public ModuleLifecycle Lifecycle { get; set; }
}