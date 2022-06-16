using System;
using System.Collections.Generic;
using System.Linq;
using Neuron.Core.Logging.Processing;
using Neuron.Core.Logging.Utils;

namespace Neuron.Core.Logging.Diagnostics;

public static class DiagnosticTokenizer
{
    public static void Tokenize(ObjectTokenizeEvent args)
    {
        if (args.Value is not DiagnosticsError error) 
            return;
        
        if (error.Exception is DiagnosticException diagnosticException)
        {
            error.Nodes.AddRange(diagnosticException.Nodes);
        }
        
        
        foreach (var property in error.Nodes.OfType<DiagnosticProperty>())
        {
            error.Properties[property.Key] = property.Value;
        }
        
        var list = new List<LogToken>();
        foreach (var node in error.Nodes)
        {
            list.AddRange(node.Render());
        }

        if (error.Properties.Count > 0)
        {
            list.Add(new LogToken()
            {
                Message = $"{ConsoleWrapper.Header("Properties")}\n",
                Type = "Diagnostic",
                Style = new LogStyle(ConsoleColor.DarkGray, ConsoleColor.Black)
            });
            foreach (var pair in error.Properties)
            {
                list.Add(new LogToken()
                {
                    Message = $"{StringHelper.Repeat(3, " ")}{pair.Key}: ",
                    Type = "Property Key",
                    Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black)
                });
                list.Add(new LogToken()
                {
                    Message = string.Join("\n", ConsoleWrapper.WrapText(9, pair.Value?.ToString() ?? "null")).TrimStart(),
                    Type = "Property Value",
                    Style = new LogStyle(ConsoleColor.Gray, ConsoleColor.Black)
                });
            }

            list.Add(new LogToken()
            {
                Message = "\n"
            });
        }

        if (error.Exception != null)
        {
            list.Add(new LogToken()
            {
                Message = $"{ConsoleWrapper.Header("Exception")}\n",
                Type = "Exception Header",
                Style = new LogStyle(ConsoleColor.DarkGray, ConsoleColor.Black)
            });
            list.Add(new LogToken()
            {
                Message = ConsoleWrapper.WrapTextToString(3, $"{error.Exception.GetType().FullName}: {error.Exception.Message}")+ "\n",
                Type = "Error",
                Style = new LogStyle(ConsoleColor.Gray, ConsoleColor.Black)
            });
            list.Add(new LogToken()
            {
                Message = ConsoleWrapper.WrapTextToString(6,
                    StringHelper.TrimIndent(error.Exception.StackTrace)
                ) + "\n",
                Type = "StackTrace",
                Style = new LogStyle(ConsoleColor.DarkGray, ConsoleColor.Black)
            });
        }

        args.Tokens = list;
    }
}