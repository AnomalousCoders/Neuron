using System;
using System.Collections.Generic;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Plugins;
using Ninject;
using Syml;

namespace Neuron.Modules.Configs;

public class ConfigService : Service
{
    private NeuronBase _neuronBase;
    private NeuronLogger _neuronLogger;
    private IKernel _kernel;
    private ConfigsModule _module;
    private ILogger _logger;

    public ConfigService(NeuronBase neuronBase, NeuronLogger neuronLogger, IKernel kernel, ConfigsModule module)
    {
        _neuronBase = neuronBase;
        _neuronLogger = neuronLogger;
        _kernel = kernel;
        _module = module;
        _logger = neuronLogger.GetLogger<ConfigService>();
    }

    public Dictionary<string, ConfigContainer> Documents = new();

    public ConfigContainer ModuleConfigs;
    public ConfigContainer PluginConfigs;
    
    public override void Enable()
    {
        ModuleConfigs = GetContainer("modules.syml");
        PluginConfigs = GetContainer("plugins.syml");
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
        var newContainer = new ConfigContainer(_neuronBase, _neuronLogger, path);
        Documents[path] = newContainer;
        return newContainer;
    }

    public void LoadBinding(ConfigBinding binding, ConfigContainer container)
    {
        var section = (IDocumentSection)container.Get(binding.Type);
        binding.Section = section;
        _kernel.Unbind(binding.Type);
        _kernel.Bind(binding.Type).ToConstant(section).InSingletonScope().ToString();
        _logger.Verbose("Bound config section [Section] with [Type]", section, binding.Type);
    }

    public void ReloadModuleConfigs()
    {
        ModuleConfigs.Load();
        var modules = _kernel.Get<ModuleManager>();
        foreach (var context in modules.GetAllModules())
        {
            foreach (var binding in context.MetaBindings.OfType<ConfigBinding>())
            {
                LoadBinding(binding, ModuleConfigs);
            }
        }
    }

    public void ReloadPluginConfigs()
    {
        PluginConfigs.Load();
        var modules = _kernel.Get<PluginManager>();
        foreach (var context in modules.Plugins)
        {
            foreach (var binding in context.MetaBindings.OfType<ConfigBinding>())
            {
                LoadBinding(binding, PluginConfigs);
            }
        }
    }

    public override void Disable()
    {
        
    }
}