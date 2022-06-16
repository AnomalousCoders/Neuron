using System;
using System.Collections.Generic;
using Neuron.Core.Logging.Processing;

namespace Neuron.Core.Logging.Diagnostics;

public class ErrorSummary : IDiagnosticNode
{
    public IEnumerable<LogToken> Render()
    {
        return new[]
        {
            new LogToken()
            {
                Message = Text + "\n",
                Type = "Diagnostic Summary",
                Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black)
            }
        };
    }

    public string Text { get; }

    public ErrorSummary(string text)
    {
        Text = text;
    }
}