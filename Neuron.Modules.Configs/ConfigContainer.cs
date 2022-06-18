using System;
using System.IO;
using System.Reflection;
using System.Text;
using Neuron.Core;
using Syml;

namespace Neuron.Modules.Configs;

public class ConfigContainer
{
    public string Path { get; set; }
    public SymlDocument Document { get; set; } = new();

    private NeuronBase _neuronBase;

    public ConfigContainer(NeuronBase neuronBase, string path)
    {
        _neuronBase = neuronBase;
        Path = path;
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

        var newSection = Activator.CreateInstance(type);
        Document.Set(newSection);
        Store();
        return newSection;
    }

    
    public void Load()
    {
        if (!_neuronBase.Platform.Configuration.FileIo) return;
        var file = _neuronBase.RelativePath(Path);
        if (!File.Exists(file)) Store();
        Document.Load(File.ReadAllText(file, Encoding.UTF8));
    }

    public void Store()
    {        
        if (!_neuronBase.Platform.Configuration.FileIo) return;
        var file = _neuronBase.RelativePath(Path);
        File.WriteAllText(file, Document.Dump(), Encoding.UTF8);
    }
    
    public void LoadString(string content) => Document.Load(content);
    public string StoreString() => Document.Dump();
}