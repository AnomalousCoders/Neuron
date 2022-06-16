using System;
using System.Collections.Generic;
using Neuron.Core.Logging.Processing;
using Neuron.Core.Logging.Utils;

namespace Neuron.Core.Logging.Diagnostics;

public class ErrorHint : IDiagnosticNode
{
    public IEnumerable<LogToken> Render()
    {
        return new[]
        {
            new LogToken()
            {
                Message = string.Join("\n", ConsoleWrapper.WrapText(6, Text)) + "\n\n",
                Type = "Diagnostic Hint",
                Style = new LogStyle(ConsoleColor.Gray, ConsoleColor.Black)
            }
        };
    }

    public string Text { get; }

    public ErrorHint(string text)
    {
        Text = text;
    }
}