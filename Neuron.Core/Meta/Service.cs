using System;
using Neuron.Core.Logging;
using Ninject;

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


    public abstract class Service : InjectedLoggerBase, IMetaObject
    {
        public virtual void Enable() { }
        public virtual void Disable() { }
    }

}