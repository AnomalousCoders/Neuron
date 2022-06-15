using System.Collections.Generic;

namespace Neuron.Modules.Commands.Simple
{
    public class CommandResult
    {
        public string Response { get; set; } = "";

        public List<IAttachment> Attachments { get; set; } = new List<IAttachment>();
    }
}