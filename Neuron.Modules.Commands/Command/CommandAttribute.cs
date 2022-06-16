using System;

namespace Neuron.Modules.Commands.Command;

public class CommandAttribute : Attribute
{
    public string CommandName { get; set; }
        
    public string[] Aliases { get; set; }
        
    public string Description { get; set; }
}