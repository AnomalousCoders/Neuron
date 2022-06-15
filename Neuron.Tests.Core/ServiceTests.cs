using System;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Platform;
using Ninject;
using Xunit;
using Xunit.Abstractions;

namespace Neuron.Tests.Core
{
    public class ServiceTests
    {
        private readonly ITestOutputHelper output;
        private readonly IPlatform _neuron;

        public ServiceTests(ITestOutputHelper output)
        {
            this.output = output;
            _neuron = NeuronMinimal.DebugHook();
        }

        [Fact]
        public void Test()
        {
            var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();
            var kernel = new StandardKernel();
            kernel.BindSimple<NeuronLogger>(logger);
            
            var metaManager = new MetaManager(logger);
            var serviceManager = new ServiceManager(kernel, metaManager);
            var batch = metaManager.Analyze(new[] {typeof(ExampleService)});
            
            Assert.Equal(0, kernel.GetBindings(typeof(ExampleService)).ToArray().Length);
            Assert.Equal(0, serviceManager.Services.Count);
            var processed = batch.Process();
            Assert.Equal(0, kernel.GetBindings(typeof(ExampleService)).ToArray().Length);
            Assert.Equal(0, serviceManager.Services.Count);
            foreach (var o in processed)
            {
                if (o is ServiceRegistration registration)
                {
                    serviceManager.BindService(registration);
                }
            }
            Assert.NotNull(kernel.Get<ExampleService>());
            Assert.Equal(1, kernel.GetBindings(typeof(ExampleService)).ToArray().Length);
            Assert.Equal(1, serviceManager.Services.Count);
            
            Assert.False(ExampleService.IsEnabled);
            foreach (var service in processed.OfType<ServiceRegistration>())
            {
                var obj = (Service)kernel.Get(service.MetaType.Type);
                obj.Enable();
            }
            Assert.True(ExampleService.IsEnabled);
            foreach (var service in processed.OfType<ServiceRegistration>())
            {
                var obj = (Service)kernel.Get(service.MetaType.Type);
                obj.Disable();
            }
            Assert.False(ExampleService.IsEnabled);
        }
    }

    public class ExampleService : Service
    {
        public static bool IsEnabled;
        public override void Enable()
        {
            IsEnabled = true;
            Logger.Info("The service has been enabled!");
        }

        public override void Disable()
        {
            IsEnabled = false;
            Logger.Info("The service has been disabled!");
        }
    }
    
}