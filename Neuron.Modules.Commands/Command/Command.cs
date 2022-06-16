using Neuron.Core.Meta;

namespace Neuron.Modules.Commands.Command;

public interface ICommand
{
    public CommandAttribute Meta { get; set; }

    internal CommandResult InternalPreExecute(ICommandContext context);
        
    internal abstract CommandResult InternalExecute(ICommandContext context);
}

public abstract class Command : InjectedLoggerBase, ICommand
{
    public CommandAttribute Meta { get; set; }
    
    CommandResult ICommand.InternalPreExecute(ICommandContext context) 
        => PreExecute(context);
    CommandResult ICommand.InternalExecute(ICommandContext context)
    {
        var result = new CommandResult();
        Execute(context, ref result);
        return result;
    }

    public virtual CommandResult PreExecute(ICommandContext context)
    {
        return null;
    }
        
    public abstract void Execute(ICommandContext context, ref CommandResult result);
}

public abstract class Command<TContext> : InjectedLoggerBase, ICommand where TContext: ICommandContext 
{
    public CommandAttribute Meta { get; set; }

    CommandResult ICommand.InternalPreExecute(ICommandContext context)
    {
        if (context is not TContext tContext)
        {
            Logger.Info("Context is not right type");
            return null;
        }
        return PreExecute(tContext);
    }

    CommandResult ICommand.InternalExecute(ICommandContext context)
    {
        if (context is not TContext tContext) return null;
        var result = new CommandResult();
        Execute(tContext, ref result);
        return result;
    }
    
    public virtual CommandResult PreExecute(TContext context)
    {
        return null;
    }
        
    public abstract void Execute(TContext context, ref CommandResult result);
}