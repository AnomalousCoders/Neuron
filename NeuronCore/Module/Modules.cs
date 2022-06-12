using System;

namespace NeuronCore.Module
{

    public class ModuleManager
    {
        private NeuronBase _neuronBase;

        public ModuleManager(NeuronBase neuronBase)
        {
            _neuronBase = neuronBase;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type[] Dependencies { get; set; }
        
    }

    public interface IModule
    {
        void Load();
        void Enable();
        void Disable();
    }

    [Module(
        Name = "Test Module",
        Description = "I dont know",
        Dependencies = new []{ typeof(TestModule) }
    )]
    public class TestModule : IModule
    {
        public void Load()
        {
            
        }

        public void Enable()
        {
            
        }

        public void Disable()
        {
            
        }
    }
}