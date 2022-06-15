namespace Neuron.Modules.Commands.Simple;

public abstract class Command
{
    public CommandAttribute Meta { get; set; }
        
    public CommandResult PreExecute(ICommandContext context)
    {
        return null;
    }
        
    public abstract CommandResult Execute(ICommandContext context);
}