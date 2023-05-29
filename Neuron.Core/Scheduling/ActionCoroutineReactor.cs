using System;

namespace Neuron.Core.Scheduling;

/// <summary>
/// Coroutine reactor which is based on <see cref="Action"/> delegates.
/// </summary>
public class ActionCoroutineReactor : CoroutineReactor
{

    /// <summary>
    /// Returns the Coroutines Reactor Tick method, the return method must be run every tick
    /// </summary>
    public Action GetTickAction() => Tick;
}