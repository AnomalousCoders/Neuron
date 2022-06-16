using System.Linq;
using System.Reflection;

namespace Neuron.Core.Meta;

public class MetaProperty
{
    public PropertyInfo Property { get; set; }
    public object[] Attributes { get; set; }
    
    public bool TryGetAttribute<T>(out T output)
    {
        output = default;
        var matching = Attributes.OfType<T>().ToArray();
        if (matching.Length == 0) return false;
        output = matching[0];
        return true;
    }

    public T GetAttribute<T>() => Attributes.OfType<T>().First();
}