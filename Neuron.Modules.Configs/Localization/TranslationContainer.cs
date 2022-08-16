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
            if (local == null || !_container.Document.Sections.ContainsKey(local)) continue;
            
            var translations = _container.Document.Get<T>(local);
            translations.SetContainerReference(this);
            translations.SetLanguage(local);
            return translations;
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

    private object GetDefault(Type type)
    {
        var defaultValue = (ITranslationsUnsafeInterface)Activator.CreateInstance(type);

        if (_container.Document.Sections.ContainsKey(defaultValue.GetDefaultLanguage()))
        {
            defaultValue = (ITranslationsUnsafeInterface)_container.Document.Sections[defaultValue.GetDefaultLanguage()].Export(type);
            defaultValue.SetContainerReference(this);
            defaultValue.SetLanguage(defaultValue.GetDefaultLanguage());
            return defaultValue;
        }

        _container.Document.Set(defaultValue.GetDefaultLanguage(), defaultValue);
        _container.Store();
        defaultValue.SetContainerReference(this);
        defaultValue.SetLanguage(defaultValue.GetDefaultLanguage());
        return defaultValue;
    }

    public object Get(Type type, string locale = null)
    {
        if (_container.Document.Sections.Count == 0 || locale == null ||
            !_container.Document.Sections.ContainsKey(locale)) return GetDefault(type);

        var export = (ITranslationsUnsafeInterface)_container.Document.Sections[locale].Export(type);
        export.SetContainerReference(this);
        export.SetLanguage(locale);
        return export;

    }

    public void Load() => _container.Load();
    public void LoadString(string content) => _container.LoadString(content);
    public string StoreString() => _container.StoreString();
}