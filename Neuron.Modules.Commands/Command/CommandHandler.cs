using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neuron.Core.Logging;
using Neuron.Modules.Commands.Event;
using Ninject;

namespace Neuron.Modules.Commands.Command;

public class CommandHandler
{
    private IKernel _kernel;
    private NeuronLogger _neuronLogger;
    private ILogger _logger;

    public CommandHandler(IKernel kernel, NeuronLogger neuronLogger)
    {
        _kernel = kernel;
        _neuronLogger = neuronLogger;
        _logger = _neuronLogger.GetLogger<CommandHandler>();
    }

    public List<ICommand> Commands = new();

    public void Raise(CommandEvent commandEvent)
    {
        if(commandEvent.IsHandled) return;
        
        foreach (var command in Commands)
        {
            var meta = command.Meta;

            var names = meta.Aliases.ToList();
            names.Add(meta.CommandName);

            foreach (var name in names)
            {
                if(!name.Equals(commandEvent.Context.Command, StringComparison.OrdinalIgnoreCase)) continue;

                var pre = command.InternalPreExecute(commandEvent.Context);
                if (pre != null)
                {
                    commandEvent.Result = pre;
                    commandEvent.IsHandled = true;
                    continue;
                }

                var handled = command.InternalExecute(commandEvent.Context);
                if (handled == null) continue;

                commandEvent.Result = handled;
                commandEvent.IsHandled = true;
                break;
            }
        }
    }

    public void RegisterCommand(Type type)
    {
        if (!typeof(ICommand).IsAssignableFrom(type)) return;
        var command = (ICommand)_kernel.Get(type);
        command.Meta = type.GetCustomAttribute<CommandAttribute>();
        Commands.Add(command);
    }
        
    public void RegisterCommand<TCommand>() where TCommand : ICommand => RegisterCommand(typeof(TCommand));

    public void RegisterCommand<TCommand>(TCommand command) where TCommand : ICommand
    {
        command.Meta = typeof(TCommand).GetCustomAttribute<CommandAttribute>();
        Commands.Add(command);
    }

    public void UnregisterAllCommands()
    {
        Commands.Clear();
    }
}