using System;
using System.IO;
using NeuronCore.Config;
using NeuronCore.Platform;
using Ninject;

namespace NeuronCore
{
    public abstract class NeuronBase
    {
        public IPlatform Platform { get; set; }
        public NeuronConfiguration Configuration { get; set; } = new NeuronConfiguration();
        internal IKernel Kernel { get; set; }
        
        public NeuronBase(IPlatform platform)
        {
            Platform = platform;
            Kernel = new StandardKernel();
            Neuron.Instance = this;
            Neuron.Kernel = Kernel;
        }

        public abstract void Start();
        public abstract void Stop();
        
        public string RelativePath(string sub) => Path.Combine(Platform.Configuration.BaseDirectory, sub);

        public string PrepareRelativeDirectory(string sub)
        {
            var dir = RelativePath(sub);
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}