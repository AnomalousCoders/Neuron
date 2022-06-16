using System.IO;
using Neuron.Core.Platform;
using Syml;

namespace Neuron.Core.Config
{
    /// <summary>
    /// Configurations for neuron and modules
    /// </summary>
    public class NeuronConfiguration
    {
        /// <summary>
        /// Engine related configurations
        /// </summary>
        public EngineSection Engine { get; internal set; } = new EngineSection();
        
        /// <summary>
        /// File related configurations
        /// </summary>
        public FilesSection Files { get; internal set; } = new FilesSection();
        
        /// <summary>
        /// Logging related configurations
        /// </summary>
        public LoggingSection Logging { get; internal set; } = new LoggingSection();

        /// <summary>
        /// The backing SYML Document
        /// </summary>
        public SymlDocument Document { get; } = new SymlDocument();

        private PlatformConfiguration _configuration;

        /// <summary>
        /// Retrieves a section and stores defaults if not already existing.
        /// </summary>
        /// <typeparam name="T">the type of the document</typeparam>
        /// <returns>the parsed document section</returns>
        public T GetSection<T>() where T : IDocumentSection, new()
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