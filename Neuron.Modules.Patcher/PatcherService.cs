using System;
using System.Collections.Generic;
using HarmonyLib;
using Neuron.Core.Meta;

namespace Neuron.Modules.Patcher;

public class PatcherService : Service
{
    private PatcherModule _patcherModule;

    public PatcherService(PatcherModule patcherModule)
    {
        _patcherModule = patcherModule;
    }
    
    public Dictionary<Type, Harmony> TypeIdentifiedPatchers { get; set; }

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

    public void PatchType(Type type)
    {
        var harmonyInstance = GetPatcherInstance(type.FullName);
        harmonyInstance.CreateClassProcessor(type).Patch();
        TypeIdentifiedPatchers[type] = harmonyInstance;
    }

    public void UnPatchType(Type type)
    {
        var harmony = TypeIdentifiedPatchers[type];
        harmony.UnpatchAll(harmony.Id);
        TypeIdentifiedPatchers.Remove(type);
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
        GetPatcherInstance().UnpatchAll();
        TypeIdentifiedPatchers.Clear();
    }
    
    internal void PatchBinding(PatchClassBinding binding)
    {
        Logger.Debug($"Applied patches from {binding.Type}");
        PatchType(binding.Type);
    }

    internal void UnpatchBinding(PatchClassBinding binding)
    {
        Logger.Debug($"Undo patches from {binding.Type}");
        UnPatchType(binding.Type);
    }
}