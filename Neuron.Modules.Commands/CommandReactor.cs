using System;
using Neuron.Core.Events;

namespace Neuron.Modules.Commands
{
    public class CommandReactor : EventReactor<CommandEvent>
    {
        public CommandHandler Handler { get; } = new CommandHandler();

        public CommandReactor()
        {
            Subscribe(Handler.EventHook);
        }

        public void RegisterCommand(Type type)
        {
            Handler.RegisterCommand(type);
        }
        
        public void RegisterCommand<TCommand>()
            where TCommand : Command
        {
            Handler.RegisterCommand<TCommand>();
        }

        public void RegisterCommand<TCommand>(TCommand command)
            where TCommand : Command
        {
            Handler.RegisterCommand(command);
        }
    }
}