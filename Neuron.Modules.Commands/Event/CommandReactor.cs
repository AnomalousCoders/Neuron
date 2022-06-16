using System;
using System.Collections.Generic;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Modules.Commands.Simple;
using Ninject;

// ReSharper disable MemberCanBePrivate.Global
namespace Neuron.Modules.Commands.Event;

public class CommandReactor : EventReactor<CommandEvent>
{
    private IKernel _kernel;
    private NeuronLogger _neuronLogger;
    private ILogger _logger;

    public CommandHandler Handler { get; }

    public OnNotFound NotFoundAction = DefaultNotFound;
    
    public CommandReactor(IKernel kernel, NeuronLogger neuronLogger)
    {
        _kernel = kernel;
        _neuronLogger = neuronLogger;
        _logger = _neuronLogger.GetLogger<CommandReactor>();
        Handler = new CommandHandler(_kernel, _neuronLogger);
        Subscribe(Handler.Raise);
    }
    
    public CommandResult Invoke(ICommandContext context)
    {
        var args = new CommandEvent
        {
            Context = context,
            IsHandled = false,
            Result = new CommandResult()
        };
        Raise(args);
        var result = args.Result;
        if (!args.IsHandled) result = NotFoundAction(args);
        return result;
    }

    public void RegisterCommand(Type type) => Handler.RegisterCommand(type);

    public void RegisterCommand<TCommand>() where TCommand : ICommand => Handler.RegisterCommand<TCommand>();

    public void RegisterCommand<TCommand>(TCommand command) where TCommand : ICommand => Handler.RegisterCommand(command);

    private static CommandResult DefaultNotFound(CommandEvent args)
    {
        var result = args.Result;
        result.Response = "Command not found";
        result.StatusCode = CommandStatusCode.NotFound;
        result.Attachments = new List<IAttachment>();
        return result;
    }
    
    public delegate CommandResult OnNotFound(CommandEvent args);
}