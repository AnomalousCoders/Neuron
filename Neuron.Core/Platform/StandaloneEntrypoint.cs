using System;
using System.Collections.Generic;
using System.IO;
using Neuron.Core.Scheduling;

namespace Neuron.Core.Platform
{
    public class StandaloneEntrypoint : IPlatform
    {
        public static void Main()
        {
            var entrypoint = new StandaloneEntrypoint();
            entrypoint.Boostrap();
        }

        public PlatformConfiguration Configuration { get; set; } = new();
        public LoopingCoroutineReactor CoroutineReactor = new();
        public NeuronBase NeuronBase { get; set; }

        public void Load()
        {
            Configuration.BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Neuron");
            Configuration.FileIo = true;
            Configuration.CoroutineReactor = CoroutineReactor;
        }
        
        public void Enable()
        {
            CoroutineReactor.StartCoroutine(TestCoroutine());
        }

        public IEnumerator<float> TestCoroutine()
        {
            yield return 1f;
            throw new NullReferenceException("Null value");
        }

        public void Continue()
        {
            CoroutineReactor.Start();
        }

        public void Disable()
        {
            CoroutineReactor.Running = false;
        }
    }
}