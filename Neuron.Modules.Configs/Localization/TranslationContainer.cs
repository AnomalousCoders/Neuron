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
            _container.Store();
            defaultValue.SetContainerReference(this);
            defaultValue.SetLanguage(defaultValue.DefaultLanguage);
            return defaultValue;
        }
        
        if (locale != null && _container.Document.Sections.ContainsKey(locale))
        {
            var translations = _container.Document.Get<T>(locale);
            translations.SetContainerReference(this);
            translations.SetLanguage(locale);
            return translations;
        }

        var fallback = _container.Document.Sections.FirstOrDefault();
        var export = fallback.Value.Export<T>();
        export.SetContainerReference(this);
        export.SetLanguage(fallback.Key);
        return export;
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

    public T AddLanguage<T>(string language, T translation) where T: Translations<T>, new()
    {
        if (_container.Document.Sections.Any(x => language == x.Key))
        {
            var translations = _container.Document.Get<T>(language);
            _container.Document.Set(language, translations);
            _container.Store();
            translations.SetContainerReference(this);
            translations.SetLanguage(language);
            return translations;
        }

        _container.Document.Set(language.ToUpper(), translation);
        _container.Store();
        translation.SetContainerReference(this);
        translation.SetLanguage(language.ToUpper());
        return translation;
    }
    
    public void LoadString(string content) => _container.LoadString(content);
    public string StoreString() => _container.StoreString();
}