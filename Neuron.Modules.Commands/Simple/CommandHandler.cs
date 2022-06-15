using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neuron.Modules.Commands.Simple;

public class CommandHandler
{
    public List<Command> Commands = new();

    public void Raise(CommandEvent commandEvent)
    {
        foreach (var command in Commands)
        {
            var meta = command.Meta;

            var names = meta.Aliases.ToList();
            names.Add(meta.CommandName);

            foreach (var name in names)
            {
                if(!name.Equals(commandEvent.Context.Command, StringComparison.OrdinalIgnoreCase)) continue;

                var pre = command.PreExecute(commandEvent.Context);
                if (pre != null)
                {
                    commandEvent.Result = pre;
                    break;
                }

                commandEvent.Result = command.Execute(commandEvent.Context);
                commandEvent.IsHandled = true;
                break;
            }
        }
    }

    public void RegisterCommand(Type type)
    {
        if(!typeof(Commands.Simple.Command).IsAssignableFrom(type)) return;

        var command = (Commands.Simple.Command)Activator.CreateInstance(type);
        command.Meta = type.GetCustomAttribute<CommandAttribute>();
        Commands.Add(command);
    }
        
    public void RegisterCommand<TCommand>()
        where TCommand : Commands.Simple.Command
    {
        RegisterCommand((TCommand)Activator.CreateInstance(typeof(TCommand)));
    }

    public void RegisterCommand<TCommand>(TCommand command)
        where TCommand : Commands.Simple.Command
    {
        command.Meta = typeof(TCommand).GetCustomAttribute<CommandAttribute>();
        Commands.Add(command);
    }

    public void UnregisterAllCommands()
    {
        Commands.Clear();
    }
}