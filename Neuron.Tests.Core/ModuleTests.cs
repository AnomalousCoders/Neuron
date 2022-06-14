using System;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Module;
using Neuron.Core.Platform;
using Ninject;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Neuron.Tests.Core
{
    public class ModuleTests
    {
        private readonly ITestOutputHelper output;
        private readonly IPlatform _neuron;

        public ModuleTests(ITestOutputHelper output)
        {
            this.output = output;
            _neuron = NeuronMinimal.DebugHook(output.WriteLine);
        }

        [Fact]
        public void Test()
        {
            var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();
            var kernel = new StandardKernel();
            kernel.BindSimple(logger);
            
            var metaManager = new MetaManager(logger);
            var serviceManager = new ServiceManager(kernel, metaManager);
            var moduleManager = new ModuleManager(_neuron.NeuronBase, metaManager, logger, kernel, serviceManager);
            moduleManager.LoadModule(new []{typeof(ModuleB), typeof(ServiceB)});
            moduleManager.LoadModule(new []{typeof(ModuleA), typeof(ServiceA)}); // Out of order for test reasons

            Assert.False(moduleManager.IsLocked);
            moduleManager.ActivateModules();
            Assert.True(moduleManager.IsLocked);

            moduleManager.EnableAll();

            var moduleB = kernel.Get<ModuleB>();
            var serviceB = kernel.Get<ServiceB>();
            
            Assert.NotNull(moduleB.A);
            Assert.NotNull(serviceB.A);
            Assert.NotNull(serviceB.ServiceA);
            
            Assert.Equal(1, kernel.GetBindings(typeof(ServiceA)).Count());
            Assert.Equal(1, kernel.GetBindings(typeof(ServiceB)).Count());
            Assert.Equal(1, kernel.GetBindings(typeof(ModuleA)).Count());
            Assert.Equal(1, kernel.GetBindings(typeof(ModuleB)).Count());
            
            moduleManager.DisableAll();
            
            Assert.Equal(0, kernel.GetBindings(typeof(ServiceA)).Count());
            Assert.Equal(0, kernel.GetBindings(typeof(ServiceB)).Count());
            Assert.Equal(0, kernel.GetBindings(typeof(ModuleA)).Count());
            Assert.Equal(0, kernel.GetBindings(typeof(ModuleB)).Count());
        }
    }

    [Module(
        Name = "Module A"
    )]
    public class ModuleA : Module
    {
        public override void Load()
        {
            Logger.Information("Loaded ModuleA");
        }

        public override void Enable()
        {
            Logger.Information("Enabled ModuleA");
        }

        public override void LateEnable()
        {
            Logger.Information("Late Enabled ModuleA");
        }

        public override void Disable()
        {
            Logger.Information("Disabled ModuleA");
        }
    }

    public class ServiceA : Service
    {
        public override void Enable()
        {
            Logger.Information("Enabled ServiceA");
        }

        public override void Disable()
        {
            Logger.Information("Disabled ServiceA");
        }
    } 
    
    [Module(
        Name = "Module B",
        Dependencies = new []{typeof(ModuleA)}
    )]
    public class ModuleB : Module
    {
        [Inject]
        public ModuleA A { get; set; }

        public override void Load()
        {
            Logger.Information("Loaded ModuleB");
        }

        public override void Enable()
        {
            
            Logger.Information("Enabled ModuleB");
        }

        public override void LateEnable()
        {
            Logger.Information("Late Enabled ModuleB");
        }

        public override void Disable()
        {
            
            Logger.Information("Disabled ModuleB");
        }
    }
    
    public class ServiceB : Service
    {
        [Inject]
        public ModuleA A { get; set; }
        
        [Inject]
        public ServiceA ServiceA { get; set; }
        
        public override void Enable()
        {
            Logger.Information("Enabled ServiceB");
        }

        public override void Disable()
        {
            Logger.Information("Disabled ServiceB");
        }
    } 
    
}