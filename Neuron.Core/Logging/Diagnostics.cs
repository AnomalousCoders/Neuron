using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neuron.Core.Logging;

public class DiagnosticsError
{
    public List<IDiagnosticNode> Nodes { get; }
    public Dictionary<string, object> Properties { get; }
    public Exception Exception { get; set; }

    public DiagnosticsError(IDiagnosticNode[] nodes, Dictionary<string, object> properties)
    {
        Nodes = nodes.ToList();
        Properties = properties;
    }

    public static DiagnosticsError FromParts(params IDiagnosticNode[] nodes)
    {
        return new DiagnosticsError(nodes, new Dictionary<string, object>());
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

public abstract class DiagnosticException : Exception
{
    public IEnumerable<IDiagnosticNode> Nodes { get; }

    protected DiagnosticException() { }

    protected DiagnosticException(string message) : base(message) { }

    protected DiagnosticException(string message, IEnumerable<IDiagnosticNode> nodes) : base(message)
    {
        Nodes = nodes;
    }

    protected DiagnosticException(string message, Exception innerException) : base(message, innerException) {}
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
        
        if (error.Exception != null && error.Exception is DiagnosticException diagnosticException)
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