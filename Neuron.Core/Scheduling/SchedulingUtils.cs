using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Neuron.Core.Scheduling;

public static class SchedulingUtils
{
    
}

public static class CoroutineUtils
{

    public static IEnumerator<float> AwaitTask<T>(Task<T> task)
    {
        while (!task.IsCompleted || !task.IsCanceled || !task.IsFaulted)
        {
            yield return 0f; // Check each tick
        }
    }

    public static IEnumerator<float> Test()
    {
        var task = new HttpClient().GetStringAsync("https://google.de");
        return AwaitTask(task);
    }
    
}