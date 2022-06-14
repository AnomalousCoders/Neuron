using System;
using Neuron.Core.Logging;
using Ninject;
using Serilog;
using Serilog.Core;

namespace Neuron.Core.Meta
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceInterfaceAttribute : MetaAttributeBase
    {
        public Type ServiceType { get; set; }

        public ServiceInterfaceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public ServiceInterfaceAttribute() { }
    }


    public abstract class Service : IMetaObject
    {
        
        [Inject]
        public NeuronLogger NeuronLoggerInjected { get; set; }

        protected ILogger Logger => NeuronLoggerInjected.GetLogger(GetType());

        public virtual void Enable() { }
        public virtual void Disable() { }
    }

}