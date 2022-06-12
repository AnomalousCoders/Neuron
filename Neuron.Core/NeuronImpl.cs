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
    public class NeuronImpl : NeuronBase {

        public NeuronImpl(IPlatform platform) : base(platform) { }
        private ILogger _logger;

        public override void Start()
        {
            Kernel = new StandardKernel();
            Neuron.Hook(this);
            Neuron.Bind<NeuronBase>(this);
            if (Platform.Configuration.OverrideConsoleEncoding) Console.OutputEncoding = Encoding.UTF8;
            Configuration.Load(Platform.Configuration);
            
            var neuronLogger = Neuron.Bind<NeuronLogger>();
            _logger = neuronLogger.GetLogger<NeuronImpl>();
            _logger.Information("Starting Neuron.Core {Box}", LogBoxes.Waiting);

            var events = Neuron.Bind<EventManager>();
            var modules = Neuron.Bind<ModuleManager>();

            if (Platform.Configuration.FileIo) /* Second Line: */ PerformIo();

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