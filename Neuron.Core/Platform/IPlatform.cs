namespace Neuron.Core.Platform
{
    /// <summary>
    /// Entry point and environment related abstraction for configuring neuron.
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// Platform related configurations.
        /// </summary>
        PlatformConfiguration Configuration { get; set; }
        
        /// <summary>
        /// Injected NeuronBase reference.
        /// </summary>
        NeuronBase NeuronBase { get; set; }
        
        /// <summary>
        /// First framework methods being executed.
        /// <see cref="Configuration"/> and <see cref="Neuron.Core.NeuronBase.Configuration"/>
        /// shall only be modified in this method.
        /// </summary>
        void Load();
        
        /// <summary>
        /// Called at the end of <see cref="Neuron.Core.NeuronBase.Start"/>.
        /// All operations performed here will still count as startup operations.
        /// </summary>
        void Enable();
        
        /// <summary>
        /// Called after neuron has been enabled.
        /// All operations performed here don't count as startup operations.
        /// </summary>
        void Continue();
        
        /// <summary>
        /// Called at <see cref="Neuron.Core.NeuronBase.Stop"/>.
        /// </summary>
        void Disable();
    }

    public static class PlatformExtension
    {
        /// <summary>
        /// Creates a <see cref="NeuronImpl"/> for the specified platform and starts
        /// the neuron lifecycle.
        /// </summary>
        public static void Boostrap(this IPlatform platform)
        {
            platform.NeuronBase = new NeuronImpl(platform);
            platform.Load();
            platform.NeuronBase.Start();
        }
    }
}