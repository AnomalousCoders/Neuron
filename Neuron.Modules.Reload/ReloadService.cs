using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Meta;
using Neuron.Core.Events;
using Neuron.Modules.Configs;
using Neuron.Modules.Configs.Localization;

namespace Neuron.Modules.Reload
{
    public class ReloadService : Service
    {
        private readonly EventManager _eventManager;
        private readonly ConfigService _configService;
        private readonly TranslationService _translationService;
        public ReloadService(EventManager eventManager, ConfigService configService, TranslationService translationService)
        {
            _eventManager = eventManager;
            _configService = configService;
            _translationService = translationService;
        }

        public EventReactor<ReloadEvent> Reload { get; } = new EventReactor<ReloadEvent>();

        public override void Enable()
        {
            _eventManager.RegisterEvent(Reload);
            Reload.Subscribe(OnReload, int.MaxValue - 100);
        }

        public override void Disable()
        {
            _eventManager.UnregisterEvent(Reload);
            Reload.Unsubscribe(OnReload);
        }

        private void OnReload(ReloadEvent _)
        {
            _configService.ReloadModuleConfigs();
            _configService.ReloadPluginConfigs();
            _translationService.ReloadTranslation();
        }
    }

    public class ReloadEvent : IEvent { }
}
