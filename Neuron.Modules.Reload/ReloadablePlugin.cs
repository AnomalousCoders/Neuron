using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Plugins;
using Neuron.Core;
using Syml;
using Neuron.Modules.Configs.Localization;


namespace Neuron.Modules.Reload
{
    public class ReloadablePlugin : Plugin
    {
        public sealed override void Enable()
        {
            Globals.Get<ReloadService>().Reload.Subscribe(Reload);
            FirstSetUp();
            EnablePlugin();
        }

        public virtual void FirstSetUp() => Reload();

        public virtual void Reload(ReloadEvent _ = null) { }

        public virtual void EnablePlugin() { }
    }

    public class ReloadablePlugin<TConfig, TTranslation> : ReloadablePlugin
        where TConfig : IDocumentSection
        where TTranslation : Translations<TTranslation>, new()
    {
        public TConfig Config { get; private set; }
        public TTranslation Translation { get; private set; }

        public sealed override void FirstSetUp()
        {
            Config = Globals.Get<TConfig>();
            Translation = Globals.Get<TTranslation>();
            OnFirstSetUp();
        }

        public sealed override void Reload(ReloadEvent _ = null)
        {
            Config = Globals.Get<TConfig>();
            Translation = Globals.Get<TTranslation>();
            OnReload();
        }

        public virtual void OnReload() { }

        public virtual void OnFirstSetUp() { }
    }
}
