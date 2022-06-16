using System;
using System.Collections.Generic;
using System.IO;
using Neuron.Core.Logging;
using Neuron.Core.Scheduling;
using Ninject;

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
            var list = new List<string>();
            list.Add("Hello");
            list.Add("World");
            var logger = NeuronBase.Kernel.Get<NeuronLogger>().GetLogger<StandaloneEntrypoint>();
            logger.Fatal(list);

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