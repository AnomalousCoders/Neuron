using System.Threading;

namespace Neuron.Core.Scheduling;

/// <summary>
/// Coroutine reactor which is based on thread freezing while loop
/// </summary>
public class LoopingCoroutineReactor : CoroutineReactor
{
    /// <summary>
    /// The delay between each tick.
    /// </summary>
    public int TickRate { get; set; } = 50;
    
    /// <summary>
    /// The execution state of the reactor.
    /// </summary>
    public bool Running { get; set; } = false;
        
    public void Start()
    {
        Running = true;
        while (Running)
        {
            Tick();
            Thread.Sleep(TickRate);
        }
    }
}