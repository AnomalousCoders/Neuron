using System.IO;
using Neuron.Core.Platform;
using Syml;

namespace Neuron.Core.Config
{
    public class NeuronConfiguration
    {
        public EngineSection Engine { get; internal set; } = new EngineSection();
        public FilesSection Files { get; internal set; } = new FilesSection();
        public LoggingSection Logging { get; internal set; } = new LoggingSection();

        public SymlDocument Document { get; } = new SymlDocument();

        private PlatformConfiguration _configuration;

        public T GetRegion<T>() where T : IDocumentSection, new()
        {
            if (Document.Has<T>())
            {
                return Document.Get<T>();
            }
            var defaultInstance = new T();
            Document.Set(defaultInstance);
            Store(_configuration);
            return defaultInstance;
        }

        internal void Load(PlatformConfiguration configuration)
        {
            _configuration = configuration;
            var file = Path.Combine(configuration.BaseDirectory, "neuron.syml");
            if (!File.Exists(file)) Store(configuration); // Write defaults
            var text = File.ReadAllText(file);
            Document.Load(text);
            Engine = Document.Get<EngineSection>();
            Files = Document.Get<FilesSection>();
            Logging = Document.Get<LoggingSection>();
            Store(_configuration);
        }

        internal void Store(PlatformConfiguration configuration)
        {   
            _configuration = configuration;
            Document.Set(Engine);
            Document.Set(Files);
            Document.Set(Logging);
            var text = Document.Dump();
            File.WriteAllText(Path.Combine(configuration.BaseDirectory, "neuron.syml"), text);
        }
        
        
    }
}