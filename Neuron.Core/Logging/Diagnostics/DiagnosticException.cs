using System;
using System.Collections.Generic;

namespace Neuron.Core.Logging.Diagnostics;

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