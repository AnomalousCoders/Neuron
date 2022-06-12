using Neuron.Core.Logging;
using Ninject;
using Serilog;

namespace Neuron.Core.Meta
{
    public abstract class Service
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly ILogger logger;
        protected Service()
        {
            logger = NeuronLogger.For(GetType());
            Globals.Kernel.Get<NeuronLogger>("Test");
            Globals.Kernel.Inject(this);
        }
    }

    public class TestService : Service
    {
        [Inject]
        public NeuronBase Base { get; set; }
        
        void Test()
        {
            
        }
    }
}