using System;
using System.IO;
using System.Text;
using Neuron.Core.Config;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Logging.Utils;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Platform;
using Neuron.Core.Plugins;
using Neuron.Core.Scheduling;
using Ninject;

namespace Neuron.Core
{
    /// <summary>
    /// Standard implementation of <see cref="NeuronBase"/>.
    /// </summary>
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
            Kernel.BindSimple(Configuration);
            
            if (Platform.Configuration.OverrideConsoleEncoding) Console.OutputEncoding = Encoding.UTF8;
            if (Platform.Configuration.FileIo)
            {
                Directory.CreateDirectory(Platform.Configuration.BaseDirectory);
                Configuration.Load(Platform.Configuration);
            }
            
            var neuronLogger = Kernel.BindSimple<NeuronLogger>();
            _logger = neuronLogger.GetLogger<NeuronImpl>();
            _logger.Info("Starting Neuron [Box]", LogBoxes.Waiting);

            var events = Kernel.BindSimple<EventManager>();
            var meta = Kernel.BindSimple<MetaManager>();
            var services = Kernel.BindSimple<ServiceManager>();
            var modules = Kernel.BindSimple<ModuleManager>();
            var plugins = Kernel.BindSimple<PluginManager>();
            var assemblies = Kernel.BindSimple<AssemblyManager>();
            
            Platform.Configuration.CoroutineReactor.Logger = neuronLogger.GetLogger<CoroutineReactor>();
            Kernel.Bind<CoroutineReactor>().ToConstant(Platform.Configuration.CoroutineReactor).InSingletonScope();
            
            if (Platform.Configuration.FileIo) LoadIoModules();
            
            Platform.Enable();
            modules.EnableAll();

            if (Platform.Configuration.FileIo) LoadIoPlugins();

            _logger.Info("Neuron started successfully [Box]", LogBoxes.Successful);
            
            Platform.Continue();
        }

        private void LoadIoModules()
        {
            _logger.Info("Loading neuron modules");
            Directory.CreateDirectory(Platform.Configuration.BaseDirectory);

            var moduleDirectory = PrepareRelativeDirectory(Configuration.Files.ModuleDirectory);
            var dependenciesDirectory = PrepareRelativeDirectory(Configuration.Files.DependenciesDirectory);

            var assemblies = Kernel.Get<AssemblyManager>();
            var moduleManager = Kernel.Get<ModuleManager>();
            var pluginManager = Kernel.Get<PluginManager>();
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

        public void LoadIoPlugins()
        {
            _logger.Info("Loading neuron plugins");
            var pluginsDirectory = PrepareRelativeDirectory(Configuration.Files.PluginDirectory);
            
            var assemblies = Kernel.Get<AssemblyManager>();
            var pluginManager = Kernel.Get<PluginManager>();
            
            foreach (var file in Directory.GetFiles(pluginsDirectory, "*.dll"))
            {
                var pluginBytes = File.ReadAllBytes(file);
                var assembly = assemblies.LoadAssembly(pluginBytes);
                var context = pluginManager.LoadPlugin(assembly.GetTypes(), assembly);
            }
        }
        
        public override void Stop()
        {
            Kernel.Get<PluginManager>().UnloadAll();
            Kernel.Get<ModuleManager>().DisableAll();
            Platform.Disable();
            Configuration.Store(Platform.Configuration); // Save latest updates
        }
    }
}