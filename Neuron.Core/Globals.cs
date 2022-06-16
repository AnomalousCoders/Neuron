using Neuron.Core.Logging;
using Neuron.Core.Logging.Utils;
using Neuron.Core.Platform;
using Ninject;

namespace Neuron.Core
{
    public static class Globals
    {
        public static NeuronBase Instance;
        public static IKernel Kernel;
        internal static void Hook(NeuronBase neuron)
        {
            Instance = neuron;
            Kernel = neuron.Kernel;
            
        }

        public static void Bind<T>(T instance) => Kernel.BindSimple(instance);

        public static T Bind<T>() => Kernel.BindSimple<T>();

        public static TA Bind<TA, TB>() where TB : TA => Kernel.BindSimple<TA, TB>();

        public static T Get<T>() => Kernel.Get<T>();
        
        public static T GetSafe<T>() => Kernel.GetSafe<T>();

        public static T Instantiate<T>() where T: new()
        {
            var instance = new T();
            Kernel.Inject(instance);
            return instance;
        }
    }

    public static class NeuronMinimal
    {
        public static IPlatform DebugHook()
        {
            var entrypoint = new PlatformDebugImpl();
            entrypoint.Boostrap();
            return entrypoint;
        }
        
        public static IPlatform DebugHook(LogMessageConsumer logConsumer)
        {
            var entrypoint = new PlatformDebugImpl();
            entrypoint.Configuration.LogEventSink = new StringEventSink(logConsumer);
            entrypoint.Boostrap();
            return entrypoint;
        }

        public static void DebugUnhook()
        {
            Globals.Kernel.Dispose();
            Globals.Instance = null;
            Globals.Kernel = null;
        }
    }
}