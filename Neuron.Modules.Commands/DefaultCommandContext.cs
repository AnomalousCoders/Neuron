using System;
using System.Linq;

namespace Neuron.Modules.Commands;

public class DefaultCommandContext : ICommandContext
{
    public string Command { get; set; }
    public string FullCommand { get; set; }
    public bool IsAdmin { get; set; }
    
    public string[] Arguments { get; set; }
    
    public Type ContextType => typeof(DefaultCommandContext);

    public static DefaultCommandContext Of(string message)
    {
        var context = new DefaultCommandContext
        {
            FullCommand = message,
            IsAdmin = true
        };
        var args = message.Split(' ').ToList();
        context.Command = args[0];
        args.RemoveAt(0);
        context.Arguments = args.ToArray();
        return context;
    }
}