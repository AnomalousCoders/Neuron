﻿using System;
using System.Runtime.InteropServices;
using Neuron.Core;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Platform;
using Neuron.Modules.Commands;
using Neuron.Modules.Commands.Command;
using Ninject;
using Xunit;
using Xunit.Abstractions;

namespace Neuron.Tests.Commands
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
            var service = new CommandService(_neuron.NeuronBase.Kernel, _neuronLogger);
            var defaultReactor = service.CreateCommandReactor();
            
            defaultReactor.RegisterCommand<ExampleCommand>();
            var result = defaultReactor.Invoke(DefaultCommandContext.Of("Example"));
            _logger.Info(result.ToString());
            Assert.NotNull(result);
            Assert.True(result.Successful);
            
            
            var result2 = defaultReactor.Invoke(DefaultCommandContext.Of("None"));
            _logger.Info(result2.ToString());
            Assert.NotNull(result2);
            Assert.False(result2.Successful);
            
            
            var result3 = defaultReactor.Invoke(DefaultCommandContext.Of("ex"));
            _logger.Info(result3.ToString());
            Assert.NotNull(result3);
            Assert.True(result3.Successful);

            var customReactor = service.CreateCommandReactor();
            customReactor.RegisterCommand<ExampleImplementation.ExampleCustomCommand>();
            var result4 = customReactor.Invoke(ExampleImplementation.CustomContext.Of("1234"),"custom");
            _logger.Info(result4.ToString());

            var result5 = customReactor.Invoke(DefaultCommandContext.Of("custom"));
            _logger.Info(result5.ToString());
        }
        
        [Fact]
        public void Test2()
        {
            var service = new CommandService(_neuron.NeuronBase.Kernel, _neuronLogger);
            var defaultReactor = service.CreateCommandReactor();
            service.GlobalHandler.RegisterCommand<ExampleCommand>();
            var result = defaultReactor.Invoke(DefaultCommandContext.Of("Example"));
            _logger.Info("[Result]", result);
            Assert.NotNull(result);
            Assert.True(result.Successful);
            var result2 = defaultReactor.Invoke(DefaultCommandContext.Of("None"));
            _logger.Info("[Result]", result2);
            Assert.NotNull(result2);
            Assert.False(result2.Successful);
        }
    }
    
    [Command(
        CommandName = "Example",
        Aliases = new[] { "ex" },
        Description = "Example Neuron Command"
    )]
    public class ExampleCommand : Command
    {
        public override void Execute(ICommandContext context, ref CommandResult result)
        {
            Logger.Info("Handling Command!");
            result.Response = "Example Command";
        }
    }

}
