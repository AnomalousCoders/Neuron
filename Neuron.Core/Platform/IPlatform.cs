namespace Neuron.Core.Platform
{
    public interface IPlatform
    {
        PlatformConfiguration Configuration { get; set; }
        NeuronBase NeuronBase { get; set; }
        void Load();
        void Enable();
        void Disable();
    }

    public static class PlatformExtension
    {
        public static void Boostrap(this IPlatform platform)
        {
            platform.NeuronBase = new NeuronImpl(platform);
            platform.Load();
            platform.NeuronBase.Start();
        }
    }
    
    public class PlatformConfiguration
    {
        /// <summary>
        ///  Base directory for all subdirectories like Modules/, Configs/, etc.
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// Disables file I/O
        /// </summary>
        public bool FileIo { get; set; } = true;

        /// <summary>
        /// Enables the override for the console encoding to UTF8
        /// </summary>
        public bool OverrideConsoleEncoding { get; set; } = true;
    }
}