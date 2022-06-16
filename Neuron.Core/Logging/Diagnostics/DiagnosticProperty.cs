using System;
using System.Collections.Generic;
using Neuron.Core.Logging.Processing;

namespace Neuron.Core.Logging.Diagnostics;

public class DiagnosticProperty : IDiagnosticNode
{
    public string Key { get; }
    public object Value { get; }

    public DiagnosticProperty(string key, object value)
    {
        Key = key;
        Value = value;
    }

    public IEnumerable<LogToken> Render() 
        => Array.Empty<LogToken>();
}