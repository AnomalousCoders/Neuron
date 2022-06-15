namespace Neuron.Modules.Commands;

public interface ICommandContext
{
    string Command { get; set; }
        
    string[] Arguments { get; set; }
        
    string FullCommand { get; set; }
}