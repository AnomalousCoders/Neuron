using System.IO;
using Neuron.Core.Events;

namespace Neuron.Core.Platform
{
    public class StandaloneEntrypoint : IPlatform
    {
        public static void Main()
        {
            var entrypoint = new StandaloneEntrypoint();
            entrypoint.Boostrap();
        }

        public PlatformConfiguration Configuration { get; set; } = new PlatformConfiguration();
        public NeuronBase NeuronBase { get; set; }

        public void Load()
        {
            Configuration.BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Neuron");
            Configuration.FileIo = true;
        }
        
        public void Enable()
        {
            new Test().Run();
        }
        
        public void Disable()
        {
            
        }
    }

    public class ExampleEventArgs : IEvent
    {
        public string Text { get; set; }
    }
}