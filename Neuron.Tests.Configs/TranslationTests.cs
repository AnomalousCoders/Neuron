using System;
using System.IO;
using System.Reflection;
using Neuron.Core;
using Neuron.Core.Dev;
using Neuron.Core.Logging;
using Neuron.Core.Platform;
using Neuron.Modules.Configs;
using Ninject;
using Syml;
using Xunit;
using Xunit.Abstractions;

namespace Neuron.Tests.Configs
{
    public class TranslationTests
    {

        private readonly ITestOutputHelper _output;
        private readonly IPlatform _neuron;
        private readonly NeuronLogger _neuronLogger;
        private readonly ILogger _logger;

        public TranslationTests(ITestOutputHelper output)
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
            var configService = new ConfigService(_neuron.NeuronBase, _neuronLogger, kernel, configsModule);
            var translationService = new TranslationService(configService, _neuron.NeuronBase, _neuronLogger, kernel, configsModule);
            var container = translationService.GetContainer("myTranslations.syml");

            var translations = (TestTranslations)container.Get(typeof(TestTranslations));
            _logger.Info(container.StoreString());
            _logger.Info(translations.HelloWorld);
            _logger.Info(translations.HelloPerson.Format("Eike"));
            Assert.True(true);
        }
        
        [Fact]
        public void Test2()
        {
            var kernel = new StandardKernel();
            kernel.BindSimple(_neuronLogger);
            var configsModule = new ConfigsModule(); // Dont inject, we just want to test the file system
            var configService = new ConfigService(_neuron.NeuronBase, _neuronLogger, kernel, configsModule);
            var translationService = new TranslationService(configService, _neuron.NeuronBase, _neuronLogger, kernel, configsModule);
            var container = translationService.GetContainer("myTranslations.syml");
            container.LoadString(@"
[ENGLISH]
helloWorld: Hello World!
helloPerson: Hello {0}!

[GERMAN]
helloWorld: Hallo Welt!
helloPerson: Hallo {0}!
");

            var translations = (TestTranslations)container.Get(typeof(TestTranslations));
            _logger.Info(container.StoreString());
            Assert.Equal("Hallo Welt!", translations.WithLocale("GERMAN").HelloWorld);
            Assert.Equal("Hello Eike!", translations.HelloPerson.Format("Eike"));
        }
    }

    public class TestTranslations : Translations<TestTranslations>
    {
        public string HelloWorld { get; set; } = "Hello World!";
        public string HelloPerson { get; set; } = "Hello {0}!";
    }

}
