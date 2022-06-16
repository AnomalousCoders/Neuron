using Neuron.Modules.Commands;

namespace Neuron.Tests.Commands.ExampleImplementation;

[CustomMeta(
    Aliases = new[] { "cc" },
    CommandName = "Custom",
    Description = "Test Command",
    Permission = "gashd"
    )]
public class ExampleCustomCommand : CustomCommand
{
    public override void Execute(CustomContext context, ref CommandResult result)
    {
        result.Response = "Executed! PlayerID: " + context.PlayerID;
        //result.StatusCode = CommandStatusCode.Ok;
    }
}