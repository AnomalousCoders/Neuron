using Serilog.Events;

namespace NeuronCore.Platform
{
    public class PlatformDebugImpl : IPlatform
    {
        public PlatformConfiguration Configuration { get; set; } = new PlatformConfiguration();
        public NeuronBase NeuronBase { get; set; }
        public void Load()
        {
            Configuration.FileIo = false;
            NeuronBase.Configuration.Logging.FileLogging = false;
            NeuronBase.Configuration.Logging.LogLevel = LogEventLevel.Debug;
        }

        public void Enable()
        {
            
        }

        public void Disable()
        {
            
        }
    }
}