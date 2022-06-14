using HarmonyLib;

namespace Neuron.Modules.Patcher;

public interface IPatcherService
{
    public Harmony Harmony { get; }
}