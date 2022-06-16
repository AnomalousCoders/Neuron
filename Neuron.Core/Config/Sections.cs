using System.ComponentModel;
using Neuron.Core.Logging;
using Syml;

namespace Neuron.Core.Config
{
    [DocumentSection("Engine")]
    public class EngineSection : IDocumentSection
    {
        [Description("Handles missing service depdencies gracefuly, I.e. loading the service anyways, or canceling the service load")]
        public bool GracefulMissingServiceDependencies { get; set; } = false;
        
        [Description("Handles missing module property depdencies gracefuly, I.e. enabling the module anyays, or canceling it")]
        public bool GracefulMissingModulePropertyDependencies { get; set; } = false;
    }

    [DocumentSection("Files")]
    public class FilesSection : IDocumentSection
    {
        [Description("Location for neuron module assemblies")]
        public string ModuleDirectory { get; set; } = "Modules/";

        [Description("Location for non neuron related assemblies")]
        public string DependenciesDirectory { get; set; } = "Dependencies/";
    }
    
    [DocumentSection("Logging")]
    public class LoggingSection : IDocumentSection
    {
        [Description("Change the minimal importance level for logs")]
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        [Description("Toggle writing console output to a logfile")]
        public bool FileLogging { get; set; } = true;
        
        [Description("Location and name of the logfile")]
        public string LogFile { get; set; } = "log.txt";
    }
}