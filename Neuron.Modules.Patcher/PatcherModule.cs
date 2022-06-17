using System;
using HarmonyLib;
using Neuron.Core;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
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

        public override void Load()
        {
            Logger.Info("Loading PatcherModule");
        }

        public override void Enable()
        {
            MetaManager.MetaGenerateBindings.Subscribe(GenerateBinding);
            Logger.Info("Enabling PatcherModule");
        }

        public void GenerateBinding(MetaGenerateBindingsEvent args)
        {
            if (args.MetaType.TryGetAttribute<PatchesAttribute>(out var patchesAttribute))
            {
                args.Outputs.Add(new PatchClassBinding()
                {
                    Type= args.MetaType.Type
                });
            }
        }

        public override void Disable()
        {
            Logger.Info("Disabling PatcherModule");
        }
    }
}

/// <summary>
/// Meta registration for patch classes
/// </summary>
public class PatchClassBinding
{
    public Type Type { get; set; }
}

/// <summary>
/// Marks a class as a HarmonyX patch class
/// </summary>
public class PatchesAttribute : MetaAttributeBase { }