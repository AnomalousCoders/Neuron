using System;
using System.Linq;
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
    public class ContextTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IPlatform _neuron;
        private readonly NeuronLogger _neuronLogger;
        private readonly ILogger _logger;

        public ContextTests(ITestOutputHelper output)
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
            defaultReactor.RegisterCommand<ExampleContextCommand>();
            var result = defaultReactor.Invoke(CustomContext.Of("Example", 9));
            _logger.Info(result.ToString());
            Assert.True(result.TryGetAttachment<CustomReturnAttachment>(out var attachment));
            Assert.Equal(18, attachment.ReturnedIntValue);
        }
    }

    public class CustomContext : DefaultCommandContext
    {
        public int IntValue { get; set; }

        public static CustomContext Of(string message, int value)
        {
            var context = new CustomContext()
            {
                FullCommand = message,
                IsAdmin = true,
                IntValue = value
            };
            var args = message.Split(' ').ToList();
            context.Command = args[0];
            args.RemoveAt(0);
            context.Arguments = args.ToArray();
            return context;
        }
    }

    public class CustomReturnAttachment : IAttachment
    {
        public int ReturnedIntValue { get; set; }
    }

    [Command(
        CommandName = "Example",
        Aliases = new[] {"ex"},
        Description = "Example Neuron Command"
    )]
    public class ExampleContextCommand : Command<CustomContext>
    {
        public override void Execute(CustomContext context, ref CommandResult result)
        {
            Logger.Info("Handling Command!");
            result.Response = "Example Command";
            result.Attachments.Add(new CustomReturnAttachment {ReturnedIntValue = context.IntValue * 2});
        }
        
    }
}