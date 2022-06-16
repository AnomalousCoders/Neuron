using System;
using System.Collections.Generic;
using System.Linq;
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

    public OnException ExceptionAction = DefaultNotFound;

    public OnException NullAction = DefaultNull;
    
    public CommandReactor(IKernel kernel, NeuronLogger neuronLogger)
    {
        _kernel = kernel;
        _neuronLogger = neuronLogger;
        _logger = _neuronLogger.GetLogger<CommandReactor>();
        Handler = new CommandHandler(_kernel, _neuronLogger);
        Subscribe(Handler.Raise);
    }

    public CommandResult Invoke(ICommandContext context, string message)
    {
        var args = message.Split(' ').ToList();
        context.Command = args[0];
        args.RemoveAt(0);
        context.Arguments = args.ToArray();
        return Invoke(context);
    }
    
    public CommandResult Invoke(ICommandContext context)
    {
        var args = new CommandEvent
        {
            Context = context,
            IsHandled = false,
            PreExecuteFailed = false,
            Result = new CommandResult()
        };
        Raise(args);
        var result = args.Result;
        if (!args.IsHandled && !args.PreExecuteFailed) result = ExceptionAction(args);
        if (result == null) result = NullAction(args);
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

    private static CommandResult DefaultNull(CommandEvent args)
    {
        var result = new CommandResult();
        result.Response = "Null Response";
        result.StatusCode = CommandStatusCode.BadSyntax;
        result.Attachments = new List<IAttachment>();
        return result;
    }
    
    public delegate CommandResult OnException(CommandEvent args);
}