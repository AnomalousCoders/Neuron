using System.Collections.Generic;

namespace Neuron.Core.Scheduling;

/// <summary>
/// Registration used inside <see cref="CoroutineReactor"/> and returned as handles.
/// </summary>
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