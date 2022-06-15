using System.Collections.Generic;

namespace Neuron.Modules.Commands.Simple;

public class CommandResult
{
    public int StatusCodeInt = (int) CommandStatusCode.Ok;

    public CommandStatusCode StatusCode
    {
        get => CommandStatusCodeHelper.Parse(StatusCodeInt);
        set => StatusCodeInt = (int) value;
    }
        
    public string Response { get; set; } = "";

    public List<IAttachment> Attachments { get; set; } = new();
}