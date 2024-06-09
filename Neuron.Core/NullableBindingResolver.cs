using Ninject.Activation.Providers;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuron.Core;

public class NullableBindingResolver : NinjectComponent, IMissingBindingResolver, INinjectComponent, IDisposable
{
    public class NullProvider : IProvider
    {
        public NullProvider(Type prototype)
        {
            Type = prototype;
        }

        public Type Type { get; private set; }

        public object Create(IContext context) => null;
    }

    /// <summary>
    /// Returns any bindings from the specified collection that match the specified service.
    /// </summary>
    /// <param name="bindings">
    /// The multimap of all registered bindings.
    /// </param>
    /// <param name="request">
    /// The series of matching bindings.
    /// </param>
    /// <returns>
    /// The series of matching bindings.
    /// </returns>
    public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
    {
        var service = request.Service;
        if (!TypeIsSelfBindable(service))
        {
            return Enumerable.Empty<IBinding>();
        }

        var provider = new NullProvider(service);

        return new Binding[1]
        {
            new Binding(service)
            {
                ProviderCallback = (IContext _) => provider
            }
        };
    }

    /// <summary>
    /// Returns a value indicating whether the specified service is self-bindable.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <returns>
    /// True if the type is self-bindable; otherwise false.
    /// </returns>
    protected virtual bool TypeIsSelfBindable(Type service)
    {
        if (!service.IsInterface && !service.IsAbstract && !service.IsValueType && service != typeof(string))
        {
            return !service.ContainsGenericParameters;
        }

        return false;
    }
}
