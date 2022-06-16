using System;
using Neuron.Core.Logging;
using Ninject;

namespace Neuron.Core.Meta
{
    /// <summary>
    /// Specify an alternate type to which the service should bind.
    /// Has to be a superclass or interface of the type.
    /// </summary>
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


    /// <summary>
    /// A dependency injection bound singleton which can be injected by other services inside
    /// the current module and is also published to all other modules, making it the goto
    /// for any api module.<br/><br/>
    /// 
    /// Uses dependency resolution to create the services in the right order, prevent
    /// Time Coupling and better diagnostic capabilities.<br/><br/>
    ///
    /// If depending on services which are published by foreign modules, make sure to
    /// add the module as a dependency in your module, as most likely the service won't
    /// be available otherwise or just because of beneficial loading orders and timing.<br/><br/>
    ///
    /// The service's lifecycle methods are bound to the module lifecycle.  
    /// </summary>
    public abstract class Service : InjectedLoggerBase, IMetaObject
    {
        /// <summary>
        /// Called before the <see cref="Modules.Module"/>.<see cref="Modules.Module.Enable"/>.
        /// </summary>
        public virtual void Enable() { }
        
        /// <summary>
        /// Called after the <see cref="Modules.Module"/>.<see cref="Modules.Module.Disable"/>.
        /// </summary>
        public virtual void Disable() { }
    }
}