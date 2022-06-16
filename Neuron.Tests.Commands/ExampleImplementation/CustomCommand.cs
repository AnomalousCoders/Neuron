using Neuron.Modules.Commands;
using Neuron.Modules.Commands.Command;

namespace Neuron.Tests.Commands.ExampleImplementation;

public abstract class CustomCommand : Command<CustomContext>
{
    public override CommandResult PreExecute(CustomContext context)
    {
        if (((CustomMeta)Meta).Permission == "*") return null;

        return new CommandResult()
        {
            StatusCode = CommandStatusCode.Forbidden,
            Response = "Missing Permission"
        };
    }
}