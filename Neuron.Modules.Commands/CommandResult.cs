using System.Collections.Generic;
using System.Linq;
using Neuron.Modules.Commands.Command;

namespace Neuron.Modules.Commands;

public class CommandResult
{
    public int StatusCodeInt = (int) CommandStatusCode.Ok;

    public CommandStatusCode StatusCode
    {
        get => CommandStatusCodeHelper.Parse(StatusCodeInt);
        set => StatusCodeInt = (int) value;
    }

    public bool Successful => CommandStatusCodeHelper.IsSuccessful(StatusCodeInt);
    
    public string Response { get; set; } = "";

    public List<IAttachment> Attachments { get; set; } = new();

    public bool TryGetAttachment<T>(out T value) where T : IAttachment
    {
        var found = Attachments.FirstOrDefault(x => x is T);
        if (found == null)
        {
            value = default;
            return false;
        }
        value = (T)found;
        return true;

    }

    public override string ToString() => $"[{StatusCodeInt}] {Response}";
}