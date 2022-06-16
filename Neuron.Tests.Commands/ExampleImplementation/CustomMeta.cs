using Neuron.Modules.Commands.Command;

namespace Neuron.Tests.Commands.ExampleImplementation;

public class CustomMeta : CommandAttribute
{
    public string Permission { get; set; }
}