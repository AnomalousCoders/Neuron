using System;
using Neuron.Core.Logging;
using Ninject;

namespace Neuron.Core.Meta;

public class InjectedLoggerBase
{
    /// <summary>
    /// Injectable logger property, do not use this directly.
    /// </summary>
    [Inject]
    public NeuronLogger NeuronLoggerInjected { get; set; }

    /// <summary>
    /// The automatically injected <see cref="Logger"/> instance for the current class.
    /// Tries to resolve resolves a logger using <see cref="NeuronLoggerInjected"/>.
    /// If the property is null, reverts to using <see cref="Globals"/>
    /// to retrieve a <see cref="NeuronLogger"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If both resolution strategies fail</exception>
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