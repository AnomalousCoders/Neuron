using System;
using System.IO;
using Neuron.Core;
using Neuron.Core.Logging;
using Neuron.Core.Platform;
using Neuron.Modules.Configs;
using Ninject;
using Syml;
using Xunit;
using Xunit.Abstractions;

namespace Neuron.Tests.Configs
{
    public class BasicTests
    {

        private readonly ITestOutputHelper _output;
        private readonly IPlatform _neuron;
        private readonly NeuronLogger _neuronLogger;
        private readonly ILogger _logger;

        public BasicTests(ITestOutputHelper output)
        {
            _output = output;
            _neuron = NeuronMinimal.DebugHook(output.WriteLine);
            _neuronLogger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();
            _logger = _neuronLogger.GetLogger<BasicTests>();
        }

        [Fact]
        public void Test1()
        {
            var kernel = new StandardKernel();
            kernel.BindSimple(_neuronLogger);
            var configsModule = new ConfigsModule(); // Dont inject, we just want to test the file system
            var service = new ConfigService(_neuron.NeuronBase, _neuronLogger, kernel, configsModule);
            var container1 = service.GetContainer("test1.syml");
            container1.LoadString(@"
[Test1]
name: Neuron
module: Config
version: 1
            ");
            
            Assert.NotNull(container1);
            var section1 = container1.Get<TestSection>();
            Assert.NotNull(section1);
            Assert.Equal("Neuron", section1.Name);
            Assert.Equal(@"
[Test1]
name: Neuron
module: Config
version: 1
            ".Trim().Replace("\r\n", "\n"), container1.StoreString().Trim().Replace("\r\n", "\n"));
            
            var container2 = service.GetContainer("test2.syml");
            var section2 = container2.Get<TestSection>();
            Assert.NotNull(section2);
            Assert.Equal("OtherValue", section2.Name);
        }
    }

    [DocumentSection("Test1")]
    public class TestSection : IDocumentSection
    {
        public string Name { get; set; } = "OtherValue";
        public string Module { get; set; } = "Core";
        public int Version { get; set; } = 2;
    }

}
