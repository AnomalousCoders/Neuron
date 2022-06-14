using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Neuron.Core.Scheduling
{
    public class Scheduling
    {
        
    }

    public class CoroutineReactor
    {
        
        private List<IEnumerator<float>> _coroutines = new();
        
        
        public void Tick()
        {
            var currentSeconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public IEnumerator<float> Coroutine()
        {
            yield return 1f;
        }
        
    }
}