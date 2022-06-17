using System;
using System.Collections.Generic;
using HarmonyLib;
using Neuron.Core;
using Neuron.Core.Events;
using Neuron.Core.Meta;
using Ninject;

namespace Neuron.Modules.Patcher;

public class PatcherService : Service
{
    private Dictionary<Type, Harmony> TypeIdentifiedPatchers { get; set; }

    public void PatchBinding(PatchClassBinding binding)
    {
        TypeIdentifiedPatchers[binding.Type] = new Harmony(binding.Type.FullName);
    }

    public void UnpatchBinding(PatchClassBinding binding)
    {
        var key = binding.Type;
        var harmony = TypeIdentifiedPatchers[key];
        harmony.UnpatchSelf();
        TypeIdentifiedPatchers.Remove(key);
    }

    public override void Enable()
    {
        TypeIdentifiedPatchers = new Dictionary<Type, Harmony>();
    }

    public override void Disable()
    {
        Harmony.UnpatchAll();
    }
}