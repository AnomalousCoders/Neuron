using System.ComponentModel;
using Serilog.Events;
using Syml;

namespace Neuron.Core.Config
{
    [DocumentSection("Engine")]
    public class EngineSection : IDocumentSection
    {
        
    }

    [DocumentSection("Files")]
    public class FilesSection : IDocumentSection
    {
        [Description("Location for neuron module assemblies")]
        public string ModuleDirectory { get; set; } = "Modules/";

        [Description("Location for neuron module configurations")]
        public string ConfigDirectory { get; set; } = "Config/";

        [Description("Location for non neuron related assemblies")]
        public string DependenciesDirectory { get; set; } = "Dependencies/";
    }
    
    [DocumentSection("Logging")]
    public class LoggingSection : IDocumentSection
    {
        [Description("Change the minimal importance level for logs")]
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

        [Description("Toggle writing console output to a logfile")]
        public bool FileLogging { get; set; } = true;
        
        [Description("Location and name of the logfile")]
        public string LogFile { get; set; } = "log.txt";
    }
}