using System;
using Neuron.Core.Logging;
using Ninject;

namespace Neuron.Core.Meta;

public class InjectedLoggerBase
{
    [Inject]
    public NeuronLogger NeuronLoggerInjected { get; set; }

    protected ILogger Logger
    {
        get
        {
            if (NeuronLoggerInjected == null && Globals.Kernel != null) Globals.Get<NeuronLogger>().GetLogger(GetType());
            return NeuronLoggerInjected?.GetLogger(GetType()) ?? throw new NullReferenceException(
                "Neuron Logger property has not been injected and globals aren't available");
        }
    }
}