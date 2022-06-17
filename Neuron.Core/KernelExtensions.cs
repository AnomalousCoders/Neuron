using System;
using System.Linq;
using Neuron.Core.Dependencies;
using Ninject;

namespace Neuron.Core;

/// <summary>
/// Utilities for working with ninject kernels.
/// </summary>
public static class KernelExtensions
{
    /// <summary>
    /// Binds a instance using a constant based singleton ninject binding.
    /// </summary>
    public static void BindSimple<T>(this IKernel kernel, T instance)
    {
        kernel.Bind<T>().ToConstant(instance).InSingletonScope();
    }

    /// <summary>
    /// Checks if all ninject injected properties of the specified type are bound.
    /// </summary>
    public static bool CheckDependencies(this IKernel kernel, Type type) 
        => KernelDependencyResolver.GetPropertyDependencies(type).All(dependency => kernel.GetBindings(dependency).Any());

    /// <summary>
    /// Binds a object using a constant based singleton ninject binding.
    /// The type is created by the ninject kernel making injection usable.
    /// </summary>
    public static T BindSimple<T>(this IKernel kernel)
    {
        kernel.Bind<T>().To<T>().InSingletonScope();
        return kernel.Get<T>();
    }
    
    /// <summary>
    /// Binds a object using a constant based singleton ninject binding.
    /// The type is created by the ninject kernel making injection usable.
    /// </summary>
    public static object BindSimple(this IKernel kernel, Type type)
    {
        kernel.Bind(type).To(type).InSingletonScope();
        return kernel.Get(type);
    }

    /// <summary>
    /// Binds a object using a constant based singleton ninject binding.
    /// The type is created by the ninject kernel making injection usable.
    /// </summary>
    /// <typeparam name="TA">The type that should be bound to</typeparam>
    /// <typeparam name="TB">The type of the object that should be bound</typeparam>
    public static TA BindSimple<TA, TB>(this IKernel kernel) where TB: TA
    {
        kernel.Bind<TA>().To<TB>().InSingletonScope();
        return kernel.Get<TA>();
    }

    /// <summary>
    /// Creates a object of the specified type and performs property
    /// injection on the created object.
    /// </summary>
    public static T Instantiate<T>(this IKernel kernel) where T: new()
    {
        var instance = new T();
        kernel.Inject(instance);
        return instance;
    }
    
    /// <summary>
    /// Returns an instance of the specified object by resolving it using
    /// ninject bindings. If no binding is present, this will return null.
    /// </summary>
    public static T GetSafe<T>(this IKernel kernel)
    {
        var exists = kernel.GetBindings(typeof(T)).Any();
        if (!exists) return default;
        return kernel.Get<T>();
    }
    
    /// <summary>
    /// Returns an instance of the specified object by resolving it using
    /// ninject bindings. If no binding is present, this will return null.
    /// </summary>
    public static object GetSafe(this IKernel kernel, Type type)
    {
        var exists = kernel.GetBindings(type).Any();
        if (!exists) return default;
        return kernel.Get(type);
    }
}