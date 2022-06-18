using System.Collections.Generic;
using Neuron.Core;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Ninject;
using Syml;

namespace Neuron.Modules.Configs;

public class ConfigService : Service
{
    private NeuronBase _neuronBase;
    private NeuronLogger _neuronLogger;
    private IKernel _kernel;
    private ConfigsModule _module;

    public ConfigService(NeuronBase neuronBase, NeuronLogger neuronLogger, IKernel kernel, ConfigsModule module)
    {
        _neuronBase = neuronBase;
        _neuronLogger = neuronLogger;
        _kernel = kernel;
        _module = module;
    }

    public Dictionary<string, ConfigContainer> Documents = new();

    public ConfigContainer ModuleConfigs;
    public ConfigContainer PluginConfigs;
    
    public override void Enable()
    {
        ModuleConfigs = GetContainer(_neuronBase.RelativePath("modules.syml"));
        PluginConfigs = GetContainer(_neuronBase.RelativePath("plugins.syml"));
        ModuleConfigs.Load();
        PluginConfigs.Load();
        while (_module.ModuleConfigBindingQueue.Count != 0)
        {
            var binding = _module.ModuleConfigBindingQueue.Dequeue();
            LoadBinding(binding, ModuleConfigs);
        }
    }

    public ConfigContainer GetContainer(string path)
    {
        if (!path.EndsWith(".syml")) path += ".syml";
        if (Documents.TryGetValue(path, out var container)) return container;
        var newContainer = new ConfigContainer(_neuronBase, path);
        Documents[path] = newContainer;
        return newContainer;
    }

    public void LoadBinding(ConfigBinding binding, ConfigContainer container)
    {
        var section = (IDocumentSection)container.Get(binding.Type);
        binding.Section = section;
        _kernel.BindSimple(section);
    }

    public override void Disable()
    {
        
    }
}