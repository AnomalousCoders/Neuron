using System;
using Neuron.Modules.Commands;

namespace Neuron.Tests.Commands.ExampleImplementation;

public class CustomContext : ICommandContext
{
    public string Command { get; set; }
    public string[] Arguments { get; set; }
    public string FullCommand { get; set; }
    public bool IsAdmin { get; set; }
    public Type ContextType => typeof(CustomContext);
    public string PlayerID { get; set; }

    public static CustomContext Of(string playerId)
    {
        var context = new CustomContext
        {
            PlayerID = playerId
        };

        return context;
    }
}