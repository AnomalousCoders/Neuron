using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neuron.Modules.Commands.Simple
{
    public class CommandHandler
    {
        public List<Command> Commands = new List<Command>();

        public void Raise(CommandEvent commandEvent)
        {
            foreach (var command in Commands)
            {
                var meta = command.Meta;

                var names = meta.Aliases.ToList();
                names.Add(meta.CommandName);

                foreach (var name in names)
                {
                    if(!name.Equals(commandEvent.Context.Command,StringComparison.OrdinalIgnoreCase)) continue;
                    
                    if(!command.PreExecute(commandEvent.Context)) continue;

                    commandEvent.Result = command.Execute(commandEvent.Context);
                    commandEvent.IsHandled = true;
                }
            }
        }

        public void RegisterCommand(Type type)
        {
            if(!typeof(Command).IsAssignableFrom(type)) return;

            var command = (Command)Activator.CreateInstance(type);
            command.Meta = type.GetCustomAttribute<CommandAttribute>();
            Commands.Add(command);
        }
        
        public void RegisterCommand<TCommand>()
            where TCommand : Command
        {
            RegisterCommand((TCommand)Activator.CreateInstance(typeof(TCommand)));
        }

        public void RegisterCommand<TCommand>(TCommand command)
            where TCommand : Command
        {
            command.Meta = typeof(TCommand).GetCustomAttribute<CommandAttribute>();
            Commands.Add(command);
        }

        public void UnregisterAllCommands()
        {
            Commands.Clear();
        }
    }
}