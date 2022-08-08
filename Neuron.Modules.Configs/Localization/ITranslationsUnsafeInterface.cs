namespace Neuron.Modules.Configs.Localization;

internal interface ITranslationsUnsafeInterface
{
    void SetContainerReference(TranslationContainer container);
    void SetLanguage(string language);
    string GetDefaultLanguage();
}