using System;
using System.Collections.Generic;
using Neuron.Core.Logging.Processing;
using Neuron.Core.Logging.Utils;

namespace Neuron.Core.Logging.Diagnostics;

public class ErrorDescription : IDiagnosticNode
{
    public IEnumerable<LogToken> Render()
    {
        return new[]
        {
            new LogToken()
            {
                Message = string.Join("\n", ConsoleWrapper.WrapText(3, Text)) + "\n\n",
                Type = "Diagnostic Description",
                Style = new LogStyle(ConsoleColor.Gray, ConsoleColor.Black)
            }
        };
    }

    public string Text { get; }

    public ErrorDescription(string text)
    {
        Text = text;
    }
}