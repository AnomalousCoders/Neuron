using System.Collections.Generic;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Plugins;
using Neuron.Modules.Configs.Localization;
using Neuron.Modules.Configs;
using Ninject;
using Syml;

namespace Neuron.Modules.Reload
{
    [Module(
        Name = "Reload",
        Description = "Reload Module",
        Author = "Dimenzio",
        Dependencies = new[]
        {
            typeof(ConfigsModule)
        }
        )]
    public class ReloadModule : Module
    {
        [Inject]
        public ReloadService ReloadService { get; set; }

        public override void Enable()
        {
            Logger.Info("Reload Module enabled");
        }

        public void Reload() => ReloadService.Reload.Raise(new ReloadEvent());
    }
}
