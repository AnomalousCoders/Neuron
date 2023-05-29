using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Neuron.Core.Scheduling;

public static class SchedulingUtils
{
    
}

public static class CoroutineUtils
{
    /// <summary>
    /// Waits until task is Completed, Canceled or Faulted.
    /// </summary>
    public static IEnumerator<float> AwaitTask<T>(Task<T> task)
    {
        while (!task.IsCompleted || !task.IsCanceled || !task.IsFaulted)
        {
            yield return 0f; // Check each tick
        }
    }

    /// <summary>
    /// Wait until <paramref name="condition"/> return <see langword="true"/>.
    /// </summary>
    /// <param name="condition">The blocking condition</param>
    public static IEnumerator<float> AwaitFor(Func<bool> condition)
    {
        while (!condition())
        {
            yield return 0f; // Check each tick
        }
    }
}