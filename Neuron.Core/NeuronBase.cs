using System.IO;
using Neuron.Core.Config;
using Neuron.Core.Platform;
using Ninject;

namespace Neuron.Core
{
    /// <summary>
    /// Base class of the neuron system.
    /// </summary>
    public abstract class NeuronBase
    {
        public IPlatform Platform { get; set; }
        public NeuronConfiguration Configuration { get; set; } = new NeuronConfiguration();
        public IKernel Kernel { get; set; }
        
        public NeuronBase(IPlatform platform)
        {
            Platform = platform;
            Kernel = new StandardKernel();
            Globals.Instance = this;
            Globals.Kernel = Kernel;
        }

        public abstract void Start();
        public abstract void Stop();

        /// <summary>
        /// Saves the current <see cref="NeuronConfiguration"/>.
        /// </summary>
        public void SaveConfig()
            => Configuration.Store(Platform.Configuration);
        
        
        /// <summary>
        /// Reloads the current <see cref="NeuronConfiguration"/>.
        /// </summary>
        public void ReloadConfig() 
            => Configuration.Load(Platform.Configuration);
        
        public string RelativePath(string sub) 
            => Path.Combine(Platform.Configuration.BaseDirectory, sub);

        public string PrepareRelativeDirectory(string sub)
        {
            var dir = RelativePath(sub);
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}