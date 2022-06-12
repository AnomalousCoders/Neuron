using System;
using System.IO;
using System.Text;
using NeuronCore.Events;
using NeuronCore.Logging;
using NeuronCore.Platform;
using Ninject;
using Ninject.Activation;
using Serilog;
using Serilog.Core;

namespace NeuronCore
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
            _logger.Information("Starting NeuronCore {Box}", LogBoxes.Waiting);

            var events = Neuron.Bind<EventManager>();

            if (Platform.Configuration.FileIo) /* Second Line: */ PerformIo();

            Platform.Enable();
            _logger.Information("NeuronCore started successfully {Box}", LogBoxes.Successful);
        }

        private void PerformIo()
        {
            _logger.Debug("NeuronCore I/O tasks {Box}", LogBoxes.Waiting);
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