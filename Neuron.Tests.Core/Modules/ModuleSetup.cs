using System;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Ninject;

namespace Neuron.Tests.Core.Modules;


    [Module(
        Name = "Module A"
    )]
    public class ModuleA : Module
    {

        public override void Load()
        {
            Logger.Info("Loaded ModuleA");
        }

        public override void Enable()
        {
            Logger.Info("Enabled ModuleA");
        }

        public override void LateEnable()
        {
            Logger.Info("Late Enabled ModuleA");
        }

        public override void Disable()
        {
            Logger.Info("Disabled ModuleA");
        }
    }

    public class ServiceA : Service
    {
        [Inject]
        public ServiceASub SubService { get; set; }

        public override void Enable()
        {
            Logger.Info("Enabled ServiceA");
        }

        public override void Disable()
        {
            Logger.Info("Disabled ServiceA");
        }
    } 
    
    public class ServiceASub : Service
    {
        public override void Enable()
        {
            Logger.Info("Enabled ServiceA Sub");
        }

        public override void Disable()
        {
            Logger.Info("Disabled ServiceA Sub");
        }
    } 
    
    [Module(
        Name = "Module B",
        Dependencies = new []{typeof(ModuleA)}
    )]
    public class ModuleB : Module
    {
        [Inject]
        public ModuleA A { get; set; }

        public override void Load()
        {
            Logger.Info("Loaded ModuleB");
        }

        public override void Enable()
        {
            
            Logger.Info("Enabled ModuleB");
        }

        public override void LateEnable()
        {
            Logger.Info("Late Enabled ModuleB");
        }

        public override void Disable()
        {
            
            Logger.Info("Disabled ModuleB");
        }
    }
    
    public class ServiceB : Service
    {
        [Inject]
        public ModuleA A { get; set; }
        
        [Inject]
        public ModuleB B { get; set; }
        
        [Inject]
        public ServiceA ServiceA { get; set; }

        public override void Enable()
        {
            Logger.Info("Enabled ServiceB");
        }

        public override void Disable()
        {
            Logger.Info("Disabled ServiceB");
        }
    }


    [Module(
        Name = "Module C",
        Dependencies = new[] {typeof(ModuleB), typeof(ModuleD)}
    )]
    public class ModuleC : Module { }
    
    [Module(
        Name = "Module D",
        Dependencies = new Type[0]
    )]
    public class ModuleD : Module { }
    
    public class Unbound {}