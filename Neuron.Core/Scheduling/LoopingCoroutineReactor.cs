using System.Threading;

namespace Neuron.Core.Scheduling;

public class LoopingCoroutineReactor : CoroutineReactor
{

    public int TickRate { get; set; } = 50;
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