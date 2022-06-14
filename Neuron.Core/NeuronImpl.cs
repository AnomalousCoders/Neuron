using System;
using System.IO;
using System.Reflection;
using System.Text;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
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
            _logger.Information("Starting Neuron {Box}", LogBoxes.Waiting);

            var events = Kernel.BindSimple<EventManager>();
            var meta = Kernel.BindSimple<MetaManager>();
            var services = Kernel.BindSimple<ServiceManager>();
            var modules = Kernel.BindSimple<ModuleManager>();
            var assemblies = Kernel.BindSimple<AssemblyManager>();
            
            if (Platform.Configuration.FileIo) LoadIoModules();
            
            modules.EnableAll();

            Platform.Enable();
            _logger.Information("Neuron started successfully {Box}", LogBoxes.Successful);
        }

        private void LoadIoModules()
        {
            _logger.Debug("Neuron.Core I/O tasks {Box}", LogBoxes.Waiting);
            Directory.CreateDirectory(Platform.Configuration.BaseDirectory);

            var moduleDirectory = PrepareRelativeDirectory(Configuration.Files.ModuleDirectory);
            var configDirectory = PrepareRelativeDirectory(Configuration.Files.ConfigDirectory);
            var dependenciesDirectory = PrepareRelativeDirectory(Configuration.Files.DependenciesDirectory);

            var assemblies = Kernel.Get<AssemblyManager>();
            var moduleManager = Kernel.Get<ModuleManager>();
            assemblies.SetupManager();
            
            foreach (var file in Directory.GetFiles(dependenciesDirectory, "*.dll"))
            {
                var moduleBytes = File.ReadAllBytes(file);
                var assembly = assemblies.LoadAssembly(moduleBytes);
            }
            
            foreach (var file in Directory.GetFiles(moduleDirectory, "*.dll"))
            {
                var moduleBytes = File.ReadAllBytes(file);
                var assembly = assemblies.LoadAssembly(moduleBytes);
                var context = moduleManager.LoadModule(assembly.GetTypes());
                context.Assembly = assembly;
            }
            
            moduleManager.ActivateModules();
        }

        public override void Stop()
        {
            Platform.Disable();
            Configuration.Store(Platform.Configuration); // Save latest updates
            Kernel.Get<ModuleManager>().DisableAll();
        }
    }
}