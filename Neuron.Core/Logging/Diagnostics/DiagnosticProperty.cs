using System;
using System.Collections.Generic;
using Neuron.Core.Logging.Processing;

namespace Neuron.Core.Logging.Diagnostics;

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