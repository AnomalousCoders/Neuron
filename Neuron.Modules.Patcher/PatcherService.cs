using System;
using System.Reflection;
using HarmonyLib;
using Neuron.Core.Events;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Ninject;

namespace Neuron.Modules.Patcher;

public class PatcherService : Service
{
    
    [Inject]
    public EventManager EventManager { get; set; }
    
    public Harmony Harmony { get; private set; }
    

    public override void Enable()
    {
        Harmony = new Harmony("Neuron Patcher");
    }

    public void PatchAssembly(Assembly assembly)
    {
        
    }

    public override void Disable()
    {
        Harmony.UnpatchSelf();
    }
}