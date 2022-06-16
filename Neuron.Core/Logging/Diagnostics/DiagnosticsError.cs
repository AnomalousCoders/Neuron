using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuron.Core.Logging.Diagnostics;

public class DiagnosticsError
{
    public List<IDiagnosticNode> Nodes { get; }
    public Dictionary<string, object> Properties { get; }
    public Exception Exception { get; set; }

    public DiagnosticsError(IEnumerable<IDiagnosticNode> nodes, Dictionary<string, object> properties)
    {
        Nodes = nodes.ToList();
        Properties = properties;
    }

    public static DiagnosticsError FromParts(params IDiagnosticNode[] nodes)
    {
        return new DiagnosticsError(nodes, new Dictionary<string, object>());
    }
    
    public static DiagnosticsError FromParts(Exception exception, params IDiagnosticNode[] nodes)
    {
        var diagnosticsError = new DiagnosticsError(nodes, new Dictionary<string, object>());
        diagnosticsError.Exception = exception;
        return diagnosticsError;
    }

    public static IDiagnosticNode Summary(string message)
        => new ErrorSummary(message);
    public static IDiagnosticNode Description(string message) 
        => new ErrorDescription(message);
    public static IDiagnosticNode Hint(string message) 
        => new ErrorHint(message);
    public static IDiagnosticNode Property(string key, object value) 
        => new DiagnosticProperty(key, value);
}