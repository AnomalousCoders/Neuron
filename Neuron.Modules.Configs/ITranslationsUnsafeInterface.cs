namespace Neuron.Modules.Configs;

internal interface ITranslationsUnsafeInterface
{
    void SetContainerReference(TranslationContainer container);
    string GetDefaultLanguage();
}