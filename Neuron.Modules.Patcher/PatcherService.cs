using System;
using System.Collections.Generic;
using HarmonyLib;
using HarmonyLib.Public.Patching;
using Neuron.Core;
using Neuron.Core.Events;
using Neuron.Core.Meta;
using Ninject;

namespace Neuron.Modules.Patcher;

public class PatcherService : Service
{

    private PatcherModule _patcherModule;

    public PatcherService(PatcherModule patcherModule)
    {
        _patcherModule = patcherModule;
    }
    
    private Dictionary<Type, Harmony> TypeIdentifiedPatchers { get; set; }

    public void PatchBinding(PatchClassBinding binding)
    {
        Logger.Debug($"Applied patches from {binding.Type}");
        var harmonyInstance = new Harmony(binding.Type.FullName);
        harmonyInstance.PatchAll(binding.Type);
        TypeIdentifiedPatchers[binding.Type] = harmonyInstance;
    }

    public void UnpatchBinding(PatchClassBinding binding)
    {
        Logger.Debug($"Undo patches from {binding.Type}");
        var key = binding.Type;
        var harmony = TypeIdentifiedPatchers[key];
        harmony.UnpatchSelf();
        TypeIdentifiedPatchers.Remove(key);
    }

    public Harmony GetPatcherInstance(string name)
    {
        var harmony = new Harmony(name);
        return harmony;
    }
    
    public Harmony GetPatcherInstance()
    {
        var guid = Guid.NewGuid();
        var harmony = new Harmony(guid.ToString());
        return harmony;
    }

    public override void Enable()
    {
        TypeIdentifiedPatchers = new Dictionary<Type, Harmony>();
        while (_patcherModule.ModuleBindingQueue.Count != 0)
        {
            var binding = _patcherModule.ModuleBindingQueue.Dequeue();
            PatchBinding(binding);
        }
    }

    public override void Disable()
    {
        Harmony.UnpatchAll();
        TypeIdentifiedPatchers.Clear();
    }
}