﻿using System.Collections.Generic;
using System.Linq;

namespace Neuron.Core.Meta;

/// <summary>
/// Result of an batch analysis of types.
/// Usually created by analyzing module assemblies.
/// </summary>
public class MetaBatchReference
{
    internal MetaManager Reference { private get; set; }
    public List<MetaType> Types { get; internal set; }

    /// <summary>
    /// Invokes the <see cref="MetaManager"/> process event for all types included in the batch.
    /// Generates a list of bindings and registrations which can then be used for final registration.
    /// </summary>
    /// <returns></returns>
    public List<IMetaBinding> GenerateBindings() => Reference.GenerateBindings(Types);

    /// <summary>
    /// Releases the included types from <see cref="Reference"/>.
    /// </summary>
    public void Untrack() => Reference.Untrack(Types);

    /// <summary>
    /// Finds types which extend the specified type.
    /// </summary>
    public List<MetaType> WhereIs<T>() => Types.Where(x => x.Is<T>()).ToList();

    /// <summary>
    /// Finds types which are annotated with the specified attribute.
    /// </summary>
    public List<MetaType> WhereAttribute<T>() => Types.Where(x => x.TryGetAttribute<T>(out _)).ToList();
}