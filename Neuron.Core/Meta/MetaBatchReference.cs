using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Neuron.Core.Meta;

public class MetaBatchReference
{
    internal MetaManager Reference { private get; set; }
    public List<MetaType> Types { get; internal set; }

    public ArrayList Process() => Reference.Process(Types);

    public void Untrack() => Reference.Untrack(Types);

    public List<MetaType> FindAlike<T>() => Types.Where(x => x.Is<T>()).ToList();
    public List<MetaType> FindAnnotated<T>() => Types.Where(x => x.TryGetAttribute<T>(out _)).ToList();
}