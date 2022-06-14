using HarmonyLib;
using Neuron.Core.Events;
using Neuron.Core.Meta;
using Ninject;

namespace Neuron.Modules.Patcher;

[ServiceInterface(typeof(IPatcherService))]
public class PatcherService : Service, IPatcherService
{
    
    [Inject]
    public EventManager EventManager { get; set; }
    
    public Harmony Harmony { get; private set; }

    public override void Enable()
    {
        Harmony = new Harmony("Neuron Patcher");
    }

    public override void Disable()
    {
        Harmony.UnpatchAll();
    }
}