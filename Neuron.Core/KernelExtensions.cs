using System;
using System.Linq;
using Neuron.Core.Dependencies;
using Ninject;

namespace Neuron.Core;

public static class KernelExtensions
{
    
    public static void BindSimple<T>(this IKernel kernel, T instance)
    {
        kernel.Bind<T>().ToConstant(instance).InSingletonScope();
    }

    public static bool CheckDependencies(this IKernel kernel, Type type) => KernelDependencyResolver.GetPropertyDependencies(type)
        .All(dependency => kernel.GetBindings(dependency).Any());

    public static T BindSimple<T>(this IKernel kernel)
    {
        kernel.Bind<T>().To<T>().InSingletonScope();
        return kernel.Get<T>();
    }

    public static TA BindSimple<TA, TB>(this IKernel kernel) where TB: TA
    {
        kernel.Bind<TA>().To<TB>().InSingletonScope();
        return kernel.Get<TA>();
    }

    public static T Instantiate<T>(this IKernel kernel) where T: new()
    {
        var instance = new T();
        kernel.Inject(instance);
        return instance;
    }
    
    public static T GetSafe<T>(this IKernel kernel)
    {
        var exists = kernel.GetBindings(typeof(T)).Any();
        if (!exists) return default;
        return kernel.Get<T>();
    }

}