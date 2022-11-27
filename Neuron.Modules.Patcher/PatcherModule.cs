using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Neuron.Core;
using Neuron.Core.Events;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Plugins;
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
        
        [Inject]
        public MetaManager MetaManager { get; set; }
        
        [Inject]
        public ModuleManager ModuleManager { get; set; }
        
        [Inject]
        public PluginManager PluginManager { get; set; }

        internal Queue<PatchClassBinding> ModuleBindingQueue = new(); 

        public override void Load(IKernel kernel)
        {
            Logger.Info("Loading PatcherModule");
            var metaManager = kernel.GetSafe<MetaManager>();
            var pluginManager = kernel.GetSafe<PluginManager>();
            var moduleManager = kernel.GetSafe<ModuleManager>();
            metaManager.MetaGenerateBindings.Subscribe(OnGenerateBinding);
            pluginManager.PluginLoad.Subscribe(OnPluginLoad);
            pluginManager.PluginUnload.Subscribe(OnPluginUnload);
            moduleManager.ModuleLoad.Subscribe(OnModuleLoad);
        }

        public override void Enable()
        {
            Logger.Info("Enabling PatcherModule");
        }

        public override void Disable()
        {
            Logger.Info("Disabling PatcherModule");
        }

        #region Event Subscriptions
        
        private void OnModuleLoad(ModuleLoadEvent args) => args.Context.MetaBindings
            .OfType<PatchClassBinding>()
            .ToList().ForEach(binding =>
            {
                Logger.Debug("Enqueue module binding");
                ModuleBindingQueue.Enqueue(binding);
            });

        private void OnPluginLoad(PluginLoadEvent args) => args.Context.MetaBindings
            .OfType<PatchClassBinding>()
            .ToList().ForEach(Patcher.PatchBinding);
        
        private void OnPluginUnload(PluginUnloadEvent args) => args.Context.MetaBindings
            .OfType<PatchClassBinding>()
            .ToList().ForEach(Patcher.UnpatchBinding);
        
        private void OnGenerateBinding(MetaGenerateBindingsEvent args)
        {
            if (!args.MetaType.TryGetAttribute<AutomaticAttribute>(out _)) return;
            if (!args.MetaType.TryGetAttribute<HarmonyPatch>(out _)) return;
            
            Logger.Debug($"* {args.MetaType.Type} [PatchBinding]");
            args.Outputs.Add(new PatchClassBinding()
            {
                Type= args.MetaType.Type
            });
        }
        
        #endregion
    }
}

/// <summary>
/// Meta registration for patch classes
/// </summary>
public class PatchClassBinding : IMetaBinding
{
    public Type Type { get; set; }
    public IEnumerable<Type> PromisedServices => new[] {Type};
}