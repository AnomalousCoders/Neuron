using Neuron.Modules.Commands.Simple;

namespace Neuron.Tests.Commands.ExampleImplementation;

public class CustomMeta : CommandAttribute
{
    public string Permission { get; set; }
}