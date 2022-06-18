using System;
using System.IO;
using System.Reflection;
using System.Text;
using Neuron.Core;
using Neuron.Core.Logging;
using Syml;

namespace Neuron.Modules.Configs;

public class ConfigContainer
{
    public string FilePath { get; set; }
    public SymlDocument Document { get; set; } = new();

    private NeuronBase _neuronBase;
    private ILogger _logger;

    public ConfigContainer(NeuronBase neuronBase, NeuronLogger logger, string filePath)
    {
        _neuronBase = neuronBase;
        _logger = logger.GetLogger<ConfigContainer>();
        FilePath = Path.ChangeExtension(filePath, ".syml");
        Load();
    }

    public T Get<T>() where T : IDocumentSection, new()
    {
        var exist = Document.Has<T>();
        if (exist)
        {
            var section = Document.Get<T>();
            Document.Set(section);
            Store();
            return section;
        }
        var newSection = new T();
        Document.Set(newSection);
        Store();
        return newSection;
    }

    public object Get(Type type)
    {
        var documentSectionAttribute = (DocumentSectionAttribute)type.GetCustomAttribute(typeof(DocumentSectionAttribute));
        var exist = Document.Sections.ContainsKey(documentSectionAttribute.SectionName);
        if (exist)
        {
            var section = Document.Get(type);
            Document.Set(section);
            Store();
            return section;
        }
        
        _logger.Verbose($"Returning default instance for configuration container [Path]", FilePath);
        var newSection = Activator.CreateInstance(type);
        Document.Set(newSection);
        Store();
        return newSection;
    }

    
    public void Load()
    {
        if (!_neuronBase.Platform.Configuration.FileIo) return;
        var file = _neuronBase.RelativePath(FilePath);
        if (!File.Exists(file)) Store();
        _logger.Verbose($"Loading file [File]", file);
        Document.Load(File.ReadAllText(file, Encoding.UTF8));
    }

    public void Store()
    {        
        if (!_neuronBase.Platform.Configuration.FileIo) return;
        var file = _neuronBase.RelativePath(FilePath);
        _logger.Verbose($"Storing file [File]", file);
        File.WriteAllText(file, Document.Dump(), Encoding.UTF8);
    }
    
    public void LoadString(string content) => Document.Load(content);
    public string StoreString() => Document.Dump();
}