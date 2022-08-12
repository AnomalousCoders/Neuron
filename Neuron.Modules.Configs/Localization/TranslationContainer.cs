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

    public T Get<T>(params string[] locale) where T: Translations<T>, new()
    {
        if (_container.Document.Sections.Count == 0)
        {
            return GetDefault<T>();
        }

        foreach (var local in locale)
        {
            if (local != null && _container.Document.Sections.ContainsKey(local))
            {
                var translations = _container.Document.Get<T>(local);
                translations.SetContainerReference(this);
                translations.SetLanguage(local);
                return translations;
            }   
        }

        return GetDefault<T>();
    }

    private T GetDefault<T>() where T : Translations<T>, new()
    {
        var defaultValue = new T();

        if (_container.Document.Sections.ContainsKey(defaultValue.DefaultLanguage))
        {
            defaultValue = _container.Document.Get<T>(defaultValue.DefaultLanguage);
            defaultValue.SetContainerReference(this);
            defaultValue.SetLanguage(defaultValue.DefaultLanguage);
            return defaultValue;
        }
        
        _container.Document.Set(defaultValue.DefaultLanguage, defaultValue);
        _container.Store();
        defaultValue.SetContainerReference(this);
        defaultValue.SetLanguage(defaultValue.DefaultLanguage);
        return defaultValue;
    }

    public object Get(Type type, string locale = null)
    {
        if (_container.Document.Sections.Count == 0)
        {
            var defaultValue = Activator.CreateInstance(type);
            var unsafeInterface = (ITranslationsUnsafeInterface)defaultValue;
            var defaultLanguage = unsafeInterface.GetDefaultLanguage();
            unsafeInterface.SetContainerReference(this);
            unsafeInterface.SetLanguage(defaultLanguage);
            _container.Document.Set(defaultLanguage, defaultValue);
            _container.Store();
            return defaultValue;
        }
        
        if (locale != null)
        {
            if (_container.Document.Sections.ContainsKey(locale))
            {
                var export = (ITranslationsUnsafeInterface)_container.Document.Sections[locale].Export(type);
                export.SetContainerReference(this);
                export.SetLanguage(locale);
                return export;   
            }
        }

        var fallback = _container.Document.Sections.FirstOrDefault();
        var fallbackExport = (ITranslationsUnsafeInterface)fallback.Value.Export(type);
        fallbackExport.SetContainerReference(this);
        fallbackExport.SetLanguage(fallback.Key);
        return fallbackExport;
    }

    public void Load() => _container.Load();
    public void LoadString(string content) => _container.LoadString(content);
    public string StoreString() => _container.StoreString();
}