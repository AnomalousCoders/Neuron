using System;
using System.Threading;

namespace Neuron.Core.Scheduling;

/// <summary>
/// Coroutine reactor which is based on thread freezing while loop
/// </summary>
public class LoopingCoroutineReactor : CoroutineReactor
{
    /// <summary>
    /// The delay between each tick in milliscondes.
    /// </summary>
    public int TickRate { get; set; } = 50;
    
    /// <summary>
    /// The execution state of the reactor.
    /// </summary>
    public bool Running { get; set; } = false;

    /// <summary>
    /// Launches the courtine handler, must be launched in a thread
    /// </summary>
    public void Start()
    {
        Running = true;
        while (Running)
        {
            var befforCall = DateTime.Now;
            Tick();
            var deleta = TickRate - (DateTime.Now - befforCall).Milliseconds;
            Thread.Sleep(deleta);
        }
    }
}