using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Dev;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Plugins;
using Ninject;

namespace Neuron.Modules.Configs.Localization;

public class TranslationService : Service
{
    private ConfigService _configService;
    private NeuronBase _neuronBase;
    private NeuronLogger _neuronLogger;
    private ILogger _logger;
    private IKernel _kernel;
    private ConfigsModule _module;

    public Dictionary<string, TranslationContainer> Translations = new();


    public TranslationService(ConfigService configService, NeuronBase neuronBase, NeuronLogger neuronLogger, IKernel kernel, ConfigsModule module)
    {
        _configService = configService;
        _neuronBase = neuronBase;
        _neuronLogger = neuronLogger;
        _kernel = kernel;
        _module = module;
        _logger = neuronLogger.GetLogger<TranslationService>();
    }

    public override void Enable()
    {
        if (_neuronBase.Platform.Configuration.FileIo)
        {
            _neuronBase.PrepareRelativeDirectory("Translations");
        }
        
        while (_module.ModuleTranslationBindingQueue.Count != 0)
        {
            var tuple = _module.ModuleTranslationBindingQueue.Dequeue();
            LoadBinding(tuple.binding, tuple.name);
        }
    }

    public TranslationContainer GetContainer(string name)
    {
        if (Translations.ContainsKey(name))
            return Translations[name];

        var container = new TranslationContainer(_configService.GetContainer(Path.Combine("Translations",
            name.Recase(StringCasing.Snake) + ".syml")));
        Translations[name] = container;
        return container;
    }

    public void LoadBinding(TranslationBinding binding, string name)
    {
        var container = GetContainer(name);
        var translations = container.Get(binding.Type);
        binding.Translations = translations;
        _kernel.Unbind(binding.Type);
        _kernel.Bind(binding.Type).ToConstant(translations).InSingletonScope().ToString();
        _logger.Verbose($"Bound translation file [Name] to [Type]", name, binding.Type);
    }

    public override void Disable()
    {
        
    }
    
    public void ReloadTranslation()
    {
        foreach (var translation in Translations)
        {
            translation.Value.Load();
        }
        
        var modules = _kernel.Get<ModuleManager>();
        foreach (var context in modules.GetAllModules())
        {
            foreach (var binding in context.MetaBindings.OfType<TranslationBinding>())
            {
                LoadBinding(binding, context.Attribute.Name.ToLowerInvariant());
            }
        }
        
        var plugins = _kernel.Get<PluginManager>().Plugins;
        foreach (var context in plugins)
        {
            foreach (var binding in context.MetaBindings.OfType<TranslationBinding>())
            {
                LoadBinding(binding, context.Attribute.Name.ToLowerInvariant());
            }
        }
    }
}