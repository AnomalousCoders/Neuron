using System;

namespace Neuron.Core.Logging.Diagnostics;

public static class NeuronDiagnosticHinter
{
    public static void AddCommonHints(Exception exception, DiagnosticsError error)
    {
        switch (exception)
        {
            case NullReferenceException:
                error.Nodes.Add(DiagnosticsError.Hint(
                    "A NullReferenceException generally means you are trying to " +
                    "access an object, which currently doesn't exist. Make sure to check " +
                    "nullable variables and fields your are using."));
                break;
            case IndexOutOfRangeException:
                error.Nodes.Add(DiagnosticsError.Hint(
                    "A IndexOutOfRangeException generally means you are trying to " +
                    "access an index (position) inside of an array or list, that is not " +
                    "within the bounds of the collection. I.e. You are trying to access the " +
                    "3. item in an list that has just 2 items."
                ));
                break;
        }
    }
}