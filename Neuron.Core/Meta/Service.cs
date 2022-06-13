using System;
using System.Collections.Generic;
using Neuron.Core.Logging;
using Ninject;
using Serilog;
using Serilog.Core;

namespace Neuron.Core.Meta
{

    public class ServiceManager
    {

        private IKernel _kernel;
        private MetaManager _meta;
        public List<ServiceBase> Services { get; set; }

        public ServiceManager(IKernel kernel, MetaManager meta)
        {
            _kernel = kernel;
            _meta = meta;
            _meta.MetaProcess.Subscribe(MetaDelegate);
            Services = new List<ServiceBase>();
        }

        public void MetaDelegate(MetaProcessEvent args)
        {
            if (args.Type.Is<ServiceBase>())
            {
                var obj = BindService(_kernel, args.Type);
                Services.Add((ServiceBase)obj);
                args.Outputs.Add(obj);
            }
        }

        public object BindService(IKernel kernel, MetaType metaType)
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
    }
    
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceInterfaceAttribute : MetaAttributeBase
    {
        public Type ServiceType { get; set; }
    }


    public abstract class ServiceBase : IMetaObject
    {
        
        [Inject]
        public NeuronLogger NeuronLoggerInjected { get; set; }

        protected ILogger Logger => NeuronLoggerInjected.GetLogger(GetType());

        public virtual void Enable() { }
        public virtual void Disable() { }
    }

}