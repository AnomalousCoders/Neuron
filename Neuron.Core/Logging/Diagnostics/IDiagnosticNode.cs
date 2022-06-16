using System.Collections.Generic;
using Neuron.Core.Logging.Processing;

namespace Neuron.Core.Logging.Diagnostics;

public interface IDiagnosticNode
{
    public IEnumerable<LogToken> Render();
}