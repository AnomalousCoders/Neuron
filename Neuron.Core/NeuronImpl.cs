using System;
using System.IO;
using System.Text;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Module;
using Neuron.Core.Platform;
using Ninject;
using Serilog;

namespace Neuron.Core
{
    public class NeuronImpl : NeuronBase 
    {
        private ILogger _logger;
        
        public NeuronImpl(IPlatform platform) : base(platform) { }

        public override void Start()
        {
            Kernel = new StandardKernel();
            if (Platform.Configuration.UseGlobals)
            {
                if (!ReferenceEquals(Globals.Instance, this)) throw new Exception("Loading unbound NeuronImpl");
                Globals.Hook(this);
            }
            Kernel.BindSimple<NeuronBase>(this);
            if (Platform.Configuration.OverrideConsoleEncoding) Console.OutputEncoding = Encoding.UTF8;
            if (Platform.Configuration.FileIo)
            {
                Directory.CreateDirectory(Platform.Configuration.BaseDirectory);
                Configuration.Load(Platform.Configuration);
            }
            
            var neuronLogger = Kernel.BindSimple<NeuronLogger>();
            _logger = neuronLogger.GetLogger<NeuronImpl>();
            _logger.Information("Starting Neuron.Core {Box}", LogBoxes.Waiting);

            if (Platform.Configuration.FileIo) PerformIo();
            
            var events = Kernel.BindSimple<EventManager>();
            var modules = Kernel.BindSimple<ModuleManager>();

            Platform.Enable();
            _logger.Information("Neuron.Core started successfully {Box}", LogBoxes.Successful);
        }

        private void PerformIo()
        {
            _logger.Debug("Neuron.Core I/O tasks {Box}", LogBoxes.Waiting);
            Directory.CreateDirectory(Platform.Configuration.BaseDirectory);

            var moduleDirectory = PrepareRelativeDirectory(Configuration.Files.ModuleDirectory);
            var configDirectory = PrepareRelativeDirectory(Configuration.Files.ConfigDirectory);
        }

        [Obsolete("Do not invoke manually", false)]
        public override void Stop()
        {
            Platform.Disable();
            Configuration.Store(Platform.Configuration);
        }
    }
}