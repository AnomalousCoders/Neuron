using System;
using Syml;
using YamlDotNet.Serialization;

namespace Neuron.Modules.Configs.Localization;

public abstract class Translations<T> : IDocumentSection, ITranslationsUnsafeInterface where T: Translations<T>, new()
{
    protected Translations()
    {
        if (typeof(T) != GetType())
            throw new Exception("Translations subclasses must reference themselves via the generic parameter");
    }

    [YamlIgnore]
    public virtual string DefaultLanguage { get; set; } = "ENGLISH";

    private TranslationContainer _container;

    public T WithLocale(string locale) => _container.Get<T>(locale);

    public void SetContainerReference(TranslationContainer container) => _container = container;
    public string GetDefaultLanguage() => DefaultLanguage;
}