namespace Neuron.Modules.Commands;

public class DefaultCommandContext : ICommandContext
{
    public string Command { get; set; }
    public string FullCommand { get; set; }
    public string[] Arguments { get; set; }
}