using Neuron.Core;
using Neuron.Core.Module;
using Ninject;

namespace Neuron.Modules.Patcher
{
    [Module(
        Name = "Patcher",
        Description = "Neuron Patcher Module"
    )]
    public class PatcherModule : Module
    {
        
        [Inject] 
        public PatcherService Patcher { get; set; }

        public override void Load()
        {
            Logger.Info("Loading PatcherModule");
        }

        public override void Enable()
        {
            Logger.Info("Enabling PatcherModule");
        }

        public override void Disable()
        {
            Logger.Info("Disabling PatcherModule");
        }
    }
}