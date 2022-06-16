using System;
using System.Collections.Generic;
using Neuron.Core.Dependencies;
using Ninject;

namespace Neuron.Core.Meta;

/// <summary>
/// Neuron service system.
/// </summary>
public class ServiceManager
{

    private IKernel _kernel;
    private MetaManager _meta;
    public List<ServiceRegistration> Services { get; set; }

    public ServiceManager(IKernel kernel, MetaManager meta)
    {
        _kernel = kernel;
        _meta = meta;
        _meta.MetaProcess.Subscribe(MetaDelegate);
        Services = new List<ServiceRegistration>();
    }

    internal void MetaDelegate(MetaProcessEvent args)
    {
        if (args.MetaType.Is<Service>())
        {
            var serviceType = args.MetaType.Type;
            if (args.MetaType.TryGetAttribute<ServiceInterfaceAttribute>(out var serviceInterface))
            {
                serviceType = serviceInterface.ServiceType;
            }
            var obj = new ServiceRegistration()
            {
                MetaType = args.MetaType,
                ServiceType = serviceType
            };
            args.Outputs.Add(obj);
        }
    }

    
    /// <summary>
    /// Binds the service registration to the ninject kernel and the local registry.
    /// </summary>
    public ServiceRegistration BindService(ServiceRegistration registration)
    {
        _kernel.Bind(registration.ServiceType).To(registration.MetaType.Type).InSingletonScope();
        _kernel.Get(registration.ServiceType);
        Services.Add(registration);
        return registration;
    }

    
    /// <summary>
    /// Unbinds the service registration from the ninject kernel and the local registry.
    /// </summary>
    public void UnbindService(ServiceRegistration service)
    {
        if (Services.Contains(service)) Services.Remove(service);
        var serviceType = service.MetaType.Type;
        if (service.MetaType.TryGetAttribute<ServiceInterfaceAttribute>(out var serviceInterface))
        {
            serviceType = serviceInterface.ServiceType;
        }
        _kernel.Unbind(serviceType);
    }
}

/// <summary>
/// Data-Holder for service registrations.
/// </summary>
public class ServiceRegistration : SimpleDependencyHolderBase
{
    public Type ServiceType { get; set; }
    public MetaType MetaType { get; set; }

    public override IEnumerable<object> Dependencies => KernelDependencyResolver.GetPropertyDependencies(MetaType.Type);
    public override object Dependable => ServiceType;

    public override string ToString() 
        => ServiceType.Name;
}