namespace Neuron.Modules.Configs.Localization;

internal interface ITranslationsUnsafeInterface
{
    void SetContainerReference(TranslationContainer container);
    string GetDefaultLanguage();
}