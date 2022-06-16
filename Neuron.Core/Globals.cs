using System;
using Neuron.Core.Logging;
using Neuron.Core.Logging.Utils;
using Neuron.Core.Platform;
using Ninject;

namespace Neuron.Core
{
    /// <summary>
    /// Global references to neuron related objects.
    /// Can and should not be used in debug environments
    /// and modules for testability.
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Current <see cref="NeuronBase"/> instance.
        /// </summary>
        public static NeuronBase Instance;
        
        /// <summary>
        /// Current <see cref="IKernel"/> instance.
        /// </summary>
        public static IKernel Kernel;
   
        internal static void Hook(NeuronBase neuron)
        {
            Instance = neuron;
            Kernel = neuron.Kernel;
            
        }

        /// <summary>
        /// Binds a instance using a constant based singleton ninject binding.
        /// </summary>
        public static void Bind<T>(T instance) 
            => Kernel.BindSimple(instance);

        
        /// <summary>
        /// Binds a object using a constant based singleton ninject binding.
        /// The type is created by the ninject kernel making injection usable.
        /// </summary>
        public static T Bind<T>() 
            => Kernel.BindSimple<T>();

        /// <summary>
        /// Binds a object using a constant based singleton ninject binding.
        /// The type is created by the ninject kernel making injection usable.
        /// </summary>
        /// <typeparam name="TA">The type that should be bound to</typeparam>
        /// <typeparam name="TB">The type of the object that should be bound</typeparam>
        public static TA Bind<TA, TB>() where TB : TA 
            => Kernel.BindSimple<TA, TB>();

        /// <summary>
        /// Returns an instance of the specified object by either resolving it using
        /// ninject bindings (I.e. the object is already present in the ninject kernel)
        /// or by creating a new instance of the type using the ninject kernel making
        /// injection usable.
        /// </summary>
        public static T Get<T>()
            => Kernel.Get<T>();
        
        /// <summary>
        /// Returns an instance of the specified object by resolving it using
        /// ninject bindings. If no binding is present, this will return null.
        /// </summary>
        public static T GetSafe<T>()
            => Kernel.GetSafe<T>();

        /// <summary>
        /// Returns an instance of the specified object by resolving it using
        /// ninject bindings. If no binding is present, this will return null.
        /// </summary>
        public static object GetSafe(Type type) 
            => Kernel.GetSafe(type); 
        
        /// <summary>
        /// Creates a object of the specified type and performs property
        /// injection on the created object.
        /// </summary>
        [Obsolete]
        public static T Instantiate<T>() where T: new()
        {
            var instance = new T();
            Kernel.Inject(instance);
            return instance;
        }
    }

    /// <summary>
    /// Utilities for quickly creating neuron debug instances usable in unit tests.
    /// </summary>
    public static class NeuronMinimal
    {
        /// <summary>
        /// Creates a new debug instance of neuron. 
        /// </summary>
        public static IPlatform DebugHook()
        {
            var entrypoint = new PlatformDebugImpl();
            entrypoint.Boostrap();
            return entrypoint;
        }
        
        /// <summary>
        /// Creates a new debug instance of neuron with a log message sink.
        /// </summary>
        public static IPlatform DebugHook(LogMessageConsumer logConsumer)
        {
            var entrypoint = new PlatformDebugImpl();
            entrypoint.Configuration.LogEventSink = new StringEventSink(logConsumer);
            entrypoint.Boostrap();
            return entrypoint;
        }
    }
}