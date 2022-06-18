using System;
using System.Linq;

namespace Neuron.Modules.Configs.Localization;

public class TranslationContainer
{
    private ConfigContainer _container;

    public TranslationContainer(ConfigContainer container)
    {
        _container = container;
    }

    public T Get<T>(string locale = null) where T: Translations<T>, new()
    {
        if (_container.Document.Sections.Count == 0)
        {
            var defaultValue = new T();
            _container.Document.Set(defaultValue.DefaultLanguage, defaultValue);
            return defaultValue;
        }
        
        if (locale != null && _container.Document.Sections.ContainsKey(locale))
        {
            return _container.Document.Get<T>(locale);
        }

        var fallback = _container.Document.Sections.FirstOrDefault();
        return fallback.Value.Export<T>();
    }
    
    public object Get(Type type, string locale = null)
    {
        if (_container.Document.Sections.Count == 0)
        {
            var defaultValue = Activator.CreateInstance(type);
            var unsafeInterface = (ITranslationsUnsafeInterface)defaultValue;
            var defaultLanguage = unsafeInterface.GetDefaultLanguage();
            unsafeInterface.SetContainerReference(this);
            _container.Document.Set(defaultLanguage, defaultValue);
            return defaultValue;
        }
        
        if (locale != null)
        {
            if (_container.Document.Sections.ContainsKey(locale))
            {
                var export = _container.Document.Sections[locale].Export(type);
                ((ITranslationsUnsafeInterface)export).SetContainerReference(this);
                return export;   
            }
        }

        var fallback = _container.Document.Sections.FirstOrDefault();
        var fallbackExport = fallback.Value.Export(type);
        ((ITranslationsUnsafeInterface)fallbackExport).SetContainerReference(this);
        return fallbackExport;
    }
    
    public void LoadString(string content) => _container.LoadString(content);
    public string StoreString() => _container.StoreString();
}