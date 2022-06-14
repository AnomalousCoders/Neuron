using System;
using System.Collections.Generic;
using System.Reflection;
using Neuron.Core.Dependencies;
using Neuron.Core.Meta;

namespace Neuron.Core.Module;

public class ModuleLoadContext : SimpleDependencyBase
{
    public Assembly Assembly { get; set; }
    public MetaBatchReference Batch { get; set; }
    public ModuleAttribute Attribute { get; set; }
    public Type[] ModuleDependencies { get; set; }
    public Type ModuleType { get; set; }
    public Module Module { get; set; }
    public ModuleLifecycle Lifecycle { get; set; }

    public override IEnumerable<object> Dependencies => ModuleDependencies;
    public override object Dependable => ModuleType;

    public override string ToString() => ModuleType.FullName;
}