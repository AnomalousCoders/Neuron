using Neuron.Core.Events;

namespace Neuron.Modules.Commands
{
    public class CommandEvent : IEvent
    {
        public ICommandContext Context { get; set; }

        public CommandResult Result { get; set; } = new CommandResult();

        public bool IsHandled { get; set; } = false;
    }
}