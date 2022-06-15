using System;
using System.Collections.Generic;
using Neuron.Core.Dependencies;
using Ninject;

namespace Neuron.Core.Meta;

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

    public void MetaDelegate(MetaProcessEvent args)
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

    //TODO: Fix Services depending on each other
    public ServiceRegistration BindService(ServiceRegistration registration)
    {
        _kernel.Bind(registration.ServiceType).To(registration.MetaType.Type).InSingletonScope();
        _kernel.Get(registration.ServiceType);
        Services.Add(registration);
        return registration;
    }

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

public class ServiceRegistration : SimpleDependencyHolderBase
{
    public Type ServiceType { get; set; }
    public MetaType MetaType { get; set; }

    public override IEnumerable<object> Dependencies => KernelDependencyResolver.GetPropertyDependencies(MetaType.Type);
    public override object Dependable => ServiceType;

    public override string ToString() => ServiceType.Name;
}