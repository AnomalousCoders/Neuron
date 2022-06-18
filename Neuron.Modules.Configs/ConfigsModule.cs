using System.Collections.Generic;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Plugins;
using Neuron.Modules.Configs.Localization;
using Ninject;
using Syml;

namespace Neuron.Modules.Configs;

[Module(Name = "Configs", Description = "Configs Module")]
public class ConfigsModule : Module
{
    public ConfigService ConfigService { get; set; } 
        
    public TranslationService TranslationService { get; set; }

    private IKernel _kernel;

    internal Queue<ConfigBinding> ModuleConfigBindingQueue = new(); 
    internal Queue<(TranslationBinding binding, string name)> ModuleTranslationBindingQueue = new(); 
        
    public override void Load(IKernel kernel)
    {
        _kernel = kernel;
        var metaManager = kernel.Get<MetaManager>();
        var moduleManager = kernel.Get<ModuleManager>();
        var pluginManager = kernel.Get<PluginManager>();
        metaManager.MetaGenerateBindings.Subscribe(OnGenerateConfigBinding);
        metaManager.MetaGenerateBindings.Subscribe(OnGenerateTranslationsBinding);
        moduleManager.ModuleLoad.Subscribe(OnModuleLoadConfigs);
        pluginManager.PluginLoad.Subscribe(OnPluginLoadConfigs);
        moduleManager.ModuleLoad.Subscribe(OnModuleLoadTranslations);
        pluginManager.PluginLoad.Subscribe(OnPluginLoadTranslations);
    }
        
    public override void Enable()
    {
        ConfigService = _kernel.GetSafe<ConfigService>();
        TranslationService = _kernel.GetSafe<TranslationService>();
    }
        
    private void OnGenerateConfigBinding(MetaGenerateBindingsEvent args)
    {
        if (!args.MetaType.TryGetAttribute<AutomaticAttribute>(out var automaticAttribute)) return;
        if (!args.MetaType.TryGetAttribute<DocumentSectionAttribute>(out var documentSectionAttribute)) return;
        if (!args.MetaType.Is<IDocumentSection>()) return;
            
        Logger.Debug($"* {args.MetaType.Type} [ConfigBinding]");
        args.Outputs.Add(new ConfigBinding()
        {
            Type = args.MetaType.Type,
            Attribute = documentSectionAttribute
        });
    }
        
    private void OnGenerateTranslationsBinding(MetaGenerateBindingsEvent args)
    {
        if (!args.MetaType.TryGetAttribute<AutomaticAttribute>(out var automaticAttribute)) return;
        if (!args.MetaType.Is<ITranslationsUnsafeInterface>()) return;
            
        Logger.Debug($"* {args.MetaType.Type} [TranslationBinding]");
        args.Outputs.Add(new TranslationBinding()
        {
            Type = args.MetaType.Type
        });
    }
        
    private void OnModuleLoadConfigs(ModuleLoadEvent args) => args.Context.MetaBindings
        .OfType<ConfigBinding>()
        .ToList().ForEach(binding =>
        {
            Logger.Debug("Enqueue module binding");
            ModuleConfigBindingQueue.Enqueue(binding);
        });

    private void OnModuleLoadTranslations(ModuleLoadEvent args) => args.Context.MetaBindings
        .OfType<TranslationBinding>()
        .ToList().ForEach(binding =>
        {
            Logger.Debug("Enqueue module binding");
            ModuleTranslationBindingQueue.Enqueue((binding, args.Context.Attribute.Name.ToLowerInvariant()));
        });
        
    private void OnPluginLoadConfigs(PluginLoadEvent args) => args.Context.MetaBindings
        .OfType<ConfigBinding>()
        .ToList().ForEach(x=> ConfigService.LoadBinding(x, ConfigService.PluginConfigs));
        
                
    private void OnPluginLoadTranslations(PluginLoadEvent args) => args.Context.MetaBindings
        .OfType<TranslationBinding>()
        .ToList().ForEach(x=> TranslationService.LoadBinding(x, args.Context.Attribute.Name.ToLowerInvariant()));
        

    public override void Disable()
    {
            
    }
}