using System;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Platform;
using Ninject;
using Serilog;
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
            Assert.NotNull(kernel.Get<ExampleService>());
            Assert.Equal(1, kernel.GetBindings(typeof(ExampleService)).ToArray().Length);
            Assert.Equal(1, serviceManager.Services.Count);
            
            Assert.False(ExampleService.IsEnabled);
            foreach (var serviceBase in processed.OfType<ServiceBase>())
            {
                serviceBase.Enable();
            }
            Assert.True(ExampleService.IsEnabled);
            foreach (var serviceBase in processed.OfType<ServiceBase>())
            {
                serviceBase.Disable();
            }
            Assert.False(ExampleService.IsEnabled);
        }
    }

    public class ExampleService : ServiceBase
    {
        public static bool IsEnabled = false;
        public override void Enable()
        {
            IsEnabled = true;
            Logger.Information("The service has been enabled!");
        }

        public override void Disable()
        {
            IsEnabled = false;
            Logger.Information("The service has been disabled!");
        }
    }
    
}