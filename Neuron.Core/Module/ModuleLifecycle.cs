using Neuron.Core.Events;

namespace Neuron.Core.Module
{
    public class ModuleLifecycle
    {
        public readonly EventReactor<VoidEvent> EnableComponents = new();
        public readonly EventReactor<VoidEvent> DisableComponents = new();

        public readonly EventReactor<VoidEvent> Enable = new();
        public readonly EventReactor<VoidEvent> LateEnable = new();
        public readonly EventReactor<VoidEvent> Disable = new();
        
        
        public void EnableSignal()
        {
            EnableComponents.Raise();
            Enable.Raise();
        }

        public void DisableSignal()
        {
            Disable.Raise();
            DisableComponents.Raise();
        }
    }
}