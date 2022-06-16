using System.Collections.Generic;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Modules.Commands.Command;
using Neuron.Modules.Commands.Event;
using Ninject;

// ReSharper disable MemberCanBePrivate.Global
namespace Neuron.Modules.Commands;

public class CommandService : Service
{
    private IKernel _kernel;
    private NeuronLogger _neuronLogger;
    private ILogger _logger;

    public CommandHandler GlobalHandler => GlobalCommandReactor.Handler;

    public readonly List<CommandReactor> CommandReactors;
    public readonly CommandReactor GlobalCommandReactor;

    public CommandService(IKernel kernel, NeuronLogger neuronLogger)
    {
        _kernel = kernel;
        _neuronLogger = neuronLogger;
        _logger = _neuronLogger.GetLogger<CommandService>();
        GlobalCommandReactor = new CommandReactor(_kernel, _neuronLogger);
        CommandReactors = new List<CommandReactor>();
    }

    public CommandReactor CreateCommandReactor()
    {
        var reactor = new CommandReactor(_kernel, _neuronLogger);
        reactor.Subscribe(CallGlobalReactor);
        CommandReactors.Add(reactor);
        return reactor;
    }

    private void CallGlobalReactor(CommandEvent context) => GlobalCommandReactor.Raise(context);
}