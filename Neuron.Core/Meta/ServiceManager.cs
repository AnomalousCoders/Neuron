using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace Neuron.Core.Meta;

public class ServiceManager
{

    private IKernel _kernel;
    private MetaManager _meta;
    public List<Service> Services { get; set; }

    public ServiceManager(IKernel kernel, MetaManager meta)
    {
        _kernel = kernel;
        _meta = meta;
        _meta.MetaProcess.Subscribe(MetaDelegate);
        Services = new List<Service>();
    }

    public void MetaDelegate(MetaProcessEvent args)
    {
        if (args.MetaType.Is<Service>())
        {
            var obj = BindService(args.MetaType);
            Services.Add((Service)obj);
            args.Outputs.Add(obj);
        }
    }

    public object BindService(MetaType metaType)
    {
        var serviceType = metaType.Type;
        if (metaType.TryGetAttribute<ServiceInterfaceAttribute>(out var serviceInterface))
        {
            serviceType = serviceInterface.ServiceType;
        }

        _kernel.Bind(serviceType).To(metaType.Type).InSingletonScope();
        var result = _kernel.Get(serviceType);
        return result;
    }

    public void UnbindService(Service service)
    {
        if (Services.Contains(service)) Services.Remove(service);
        var metaType = MetaType.Analyze(service.GetType());
        var serviceType = metaType.Type;
        if (metaType.TryGetAttribute<ServiceInterfaceAttribute>(out var serviceInterface))
        {
            serviceType = serviceInterface.ServiceType;
        }
        _kernel.Unbind(serviceType);
    }
}