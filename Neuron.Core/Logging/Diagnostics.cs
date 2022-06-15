using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neuron.Core.Logging.Neuron;

public class DiagnosticsError
{
    public IDiagnosticNode[] Nodes { get; }
    public StackTrace Trace { get; }
    public Dictionary<string, object> Properties { get; }

    public DiagnosticsError(IDiagnosticNode[] nodes, StackTrace trace, Dictionary<string, object> properties)
    {
        Nodes = nodes;
        Trace = trace;
        Properties = properties;
    }

    public static DiagnosticsError FromParts(params IDiagnosticNode[] nodes)
    {
        var trace = new StackTrace();
        var properties = new Dictionary<string, object>();
        foreach (var property in nodes.OfType<DiagnosticProperty>())
        {
            properties[property.Key] = property.Value;
        }
        return new DiagnosticsError(nodes, trace, properties);
    }

    public static IDiagnosticNode Summary(string message) => new ErrorSummary(message);
    public static IDiagnosticNode Description(string message) => new ErrorDescription(message);
    public static IDiagnosticNode Hint(string message) => new ErrorHint(message);
    public static IDiagnosticNode Property(string key, object value) => new DiagnosticProperty(key, value);
}

public interface IDiagnosticNode
{
    public IEnumerable<LogToken> Render();
}

public class DiagnosticProperty : IDiagnosticNode
{
    public string Key;
    public object Value;

    public DiagnosticProperty(string key, object value)
    {
        Key = key;
        Value = value;
    }

    public IEnumerable<LogToken> Render() => Array.Empty<LogToken>();
}

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

public class DiagnosticTokenizer
{
    public static void Tokenize(ObjectTokenizeEvent args)
    {
        if (args.Value is not DiagnosticsError error) return;
        var list = new List<LogToken>();
        foreach (var node in error.Nodes)
        {
            list.AddRange(node.Render());
        }

        if (error.Properties.Count > 0)
        {
            list.Add(new LogToken()
            {
                Message = $"\n{ConsoleWrapper.Header("Properties")}\n",
                Type = "Diagnostic",
                Style = new LogStyle(ConsoleColor.DarkGray, ConsoleColor.Black)
            });
            foreach (var pair in error.Properties)
            {
                list.Add(new LogToken()
                {
                    Message = $"{StringHelper.Repeat(3, " ")}{pair.Key}: ",
                    Type = "Diagnostic Trace",
                    Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black)
                });
                list.Add(new LogToken()
                {
                    Message = string.Join("\n", ConsoleWrapper.WrapText(9, pair.Value?.ToString() ?? "null")).TrimStart(),
                    Type = "Diagnostic Trace",
                    Style = new LogStyle(ConsoleColor.Gray, ConsoleColor.Black)
                });
            }

            list.Add(new LogToken()
            {
                Message = "\n"
            });
        }

        list.Add(new LogToken()
        {
            Message = $"\n{ConsoleWrapper.Header("StackTrace")}\n",
            Type = "Diagnostic",
            Style = new LogStyle(ConsoleColor.DarkGray, ConsoleColor.Black)
        });
        list.Add(new LogToken()
        {
            Message = string.Join("\n", ConsoleWrapper.WrapText(3,
                string.Join("\n", error.Trace.ToString()
                    .Split('\n')
                    .Select(x => x.Trim()))
            )) + "\n",
            Type = "Diagnostic Trace",
            Style = new LogStyle(ConsoleColor.DarkGray, ConsoleColor.Black)
        });

        args.Tokens = list;
    }
}