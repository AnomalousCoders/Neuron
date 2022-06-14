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
        public IPatcherService Patcher { get; set; }

        public override void Load()
        {
            Logger.Information("Loading PatcherModule");
        }

        public override void Enable()
        {
            Logger.Information("Enabling PatcherModule");
        }

        public override void Disable()
        {
            Logger.Information("Disabling PatcherModule");
        }
    }
}