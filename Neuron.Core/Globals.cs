using Neuron.Core.Platform;
using Ninject;

namespace Neuron.Core
{
    public static class Globals
    {
        public static NeuronBase Instance;
        public static IKernel Kernel;
        public static void Hook(NeuronBase neuron)
        {
            Instance = neuron;
            Kernel = neuron.Kernel;
            
        }

        public static void Bind<T>(T instance)
        {
            Kernel.Bind<T>().ToConstant(instance).InSingletonScope();
        }
        
        public static T Bind<T>()
        {
            Kernel.Bind<T>().To<T>().InSingletonScope();
            return Get<T>();
        }
        
        public static TA Bind<TA, TB>() where TB: TA
        {
            Kernel.Bind<TA>().To<TB>().InSingletonScope();
            return Get<TA>();
        }

        public static T Get<T>() => Kernel.Get<T>();

        public static T Instantiate<T>() where T: new()
        {
            var instance = new T();
            Kernel.Inject(instance);
            return instance;
        }
    }

    public static class NeuronMinimal
    {
        public static void DebugHook()
        {
            var entrypoint = new PlatformDebugImpl();
            entrypoint.Boostrap();
        }

        public static void DebugUnhook()
        {
            Globals.Instance = null;
            Globals.Kernel = null;
        }
    }
}