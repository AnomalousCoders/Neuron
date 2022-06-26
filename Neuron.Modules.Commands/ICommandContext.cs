using System;

namespace Neuron.Modules.Commands;

public interface ICommandContext
{
    string Command { get; set; }
        
    string[] Arguments { get; set; }
        
    string FullCommand { get; set; }
    
    bool IsAdmin { get; }
    
    Type ContextType { get; }
}