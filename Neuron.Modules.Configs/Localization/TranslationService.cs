using System.IO;
using Neuron.Core;
using Neuron.Core.Dev;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
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

    public TranslationContainer GetContainer(string fileName) =>
        new(_configService.GetContainer(Path.Combine("Translations", fileName)));

    public void LoadBinding(TranslationBinding binding, string name)
    {
        var container = GetContainer($"{name.Recase(StringCasing.Snake)}.syml");
        var translations = container.Get(binding.Type);
        binding.Translations = translations;
        _kernel.Unbind(binding.Type);
        _kernel.Bind(binding.Type).ToConstant(translations).InSingletonScope().ToString();
        _logger.Verbose($"Bound translation file [Name] to [Type]", name, binding.Type);
    }

    public override void Disable()
    {
        
    }
}