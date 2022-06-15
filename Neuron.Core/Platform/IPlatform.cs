namespace Neuron.Core.Platform
{
    public interface IPlatform
    {
        PlatformConfiguration Configuration { get; set; }
        NeuronBase NeuronBase { get; set; }
        void Load();
        void Enable();
        void Continue();
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
}