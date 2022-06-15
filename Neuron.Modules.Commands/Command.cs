namespace Neuron.Modules.Commands
{
    public abstract class Command
    {
        public CommandAttribute Meta { get; set; }
        
        public virtual bool PreExecute(ICommandContext context)
        {
            return true;
        }
        
        public abstract CommandResult Execute(ICommandContext context);
    }
}