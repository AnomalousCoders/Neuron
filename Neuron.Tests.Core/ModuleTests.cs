using System;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Dependencies;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Platform;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Neuron.Tests.Core
{
    public class ModuleTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IPlatform _neuron;

        public ModuleTests(ITestOutputHelper output)
        {
            this._output = output;
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
            //moduleManager.LoadModule(new []{typeof(ModuleC)});
            moduleManager.LoadModule(new []{typeof(ModuleB), typeof(ServiceB)});
            moduleManager.LoadModule(new []{typeof(ModuleD)});
            moduleManager.LoadModule(new []{typeof(ModuleA), typeof(ServiceA), typeof(ServiceASub)}); // Out of order for test reasons
            
            _output.WriteLine(String.Join(":", KernelDependencyResolver.GetPropertyDependencies(typeof(ServiceA))));
            _output.WriteLine(String.Join(":", KernelDependencyResolver.GetPropertyDependencies(typeof(ServiceB))));
            _output.WriteLine(String.Join(":", KernelDependencyResolver.GetPropertyDependencies(typeof(ModuleB))));
            _output.WriteLine(kernel.GetBindings(typeof(Random)).Any().ToString());
            
            Assert.False(moduleManager.IsLocked);
            moduleManager.ActivateModules();
            Assert.True(moduleManager.IsLocked);

            _output.WriteLine(kernel.GetBindings(typeof(Random)).Any().ToString());
            moduleManager.EnableAll();
            _output.WriteLine(kernel.GetBindings(typeof(Random)).Any().ToString());
            
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
            Logger.Info("Loaded ModuleA");
        }

        public override void Enable()
        {
            Logger.Info("Enabled ModuleA");
        }

        public override void LateEnable()
        {
            Logger.Info("Late Enabled ModuleA");
        }

        public override void Disable()
        {
            Logger.Info("Disabled ModuleA");
        }
    }

    public class ServiceA : Service
    {
        [Inject]
        public ServiceASub SubService { get; set; }

        public override void Enable()
        {
            Logger.Info("Enabled ServiceA");
        }

        public override void Disable()
        {
            Logger.Info("Disabled ServiceA");
        }
    } 
    
    public class ServiceASub : Service
    {
        public override void Enable()
        {
            Logger.Info("Enabled ServiceA Sub");
        }

        public override void Disable()
        {
            Logger.Info("Disabled ServiceA Sub");
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
            Logger.Info("Loaded ModuleB");
        }

        public override void Enable()
        {
            
            Logger.Info("Enabled ModuleB");
        }

        public override void LateEnable()
        {
            Logger.Info("Late Enabled ModuleB");
        }

        public override void Disable()
        {
            
            Logger.Info("Disabled ModuleB");
        }
    }
    
    public class ServiceB : Service
    {
        [Inject]
        public ModuleA A { get; set; }
        
        [Inject]
        public ModuleB B { get; set; }
        
        [Inject]
        public ServiceA ServiceA { get; set; }

        public override void Enable()
        {
            Logger.Info("Enabled ServiceB");
        }

        public override void Disable()
        {
            Logger.Info("Disabled ServiceB");
        }
    }


    [Module(
        Name = "Module C",
        Dependencies = new[] {typeof(ModuleB), typeof(ModuleD)}
    )]
    public class ModuleC : Module { }
    
    [Module(
        Name = "Module D",
        Dependencies = new Type[0]
    )]
    public class ModuleD : Module { }
    
    public class Unbound {}
}