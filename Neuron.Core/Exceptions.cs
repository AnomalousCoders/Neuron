using System;

namespace Neuron.Core;

public class IndefiniteExtensionPointException: Exception
{
    public IndefiniteExtensionPointException() { }
    public IndefiniteExtensionPointException(string message) : base(message) { }
}