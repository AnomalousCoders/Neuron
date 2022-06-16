using System;

namespace Neuron.Core.Scheduling;

/// <summary>
/// Coroutine reactor which is based on <see cref="Action"/> delegates.
/// </summary>
public class ActionCoroutineReactor : CoroutineReactor
{

    public int TickRate { get; set; } = 50;
    public bool Running { get; set; } = false;

    public Action GetTickAction() 
        => Tick;
}