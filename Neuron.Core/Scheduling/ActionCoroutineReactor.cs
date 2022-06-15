using System;

namespace Neuron.Core.Scheduling;

public class ActionCoroutineReactor : CoroutineReactor
{

    public int TickRate { get; set; } = 50;
    public bool Running { get; set; } = false;

    public Action GetTickAction() => Tick;
}