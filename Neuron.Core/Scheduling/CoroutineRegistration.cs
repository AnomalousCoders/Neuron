using System.Collections.Generic;

namespace Neuron.Core.Scheduling;

public class CoroutineRegistration
{
    public IEnumerator<float> Enumerator;
    public long ScheduledUpdate;

    public CoroutineRegistration(IEnumerator<float> enumerator)
    {
        Enumerator = enumerator;
        ScheduledUpdate = 0;
    }
}