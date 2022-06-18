using System;
using System.Collections.Generic;
using Neuron.Core.Meta;
using Syml;

namespace Neuron.Modules.Configs;

public class ConfigBinding : IMetaBinding
{
    public Type Type { get; set; }
    public DocumentSectionAttribute Attribute { get; set; }
    public IDocumentSection Section { get; set; }
    public IEnumerable<Type> PromisedServices => new[] {Type};
}

public class TranslationBinding : IMetaBinding
{
    public Type Type { get; set; }
    public object Translations { get; set; }
    public IEnumerable<Type> PromisedServices => new[] {Type};
}