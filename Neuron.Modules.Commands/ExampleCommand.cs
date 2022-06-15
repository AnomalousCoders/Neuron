namespace Neuron.Modules.Commands
{
    [Command(
        CommandName = "Example",
        Aliases = new[] { "ex" },
        Description = "Example Neuron Command"
        )]
    public class ExampleCommand : Command
    {
        public override CommandResult Execute(ICommandContext context)
        {
            return new CommandResult()
            {
                Response = "Example Command!"
            };
        }
    }
}