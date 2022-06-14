using System.Collections.Generic;

namespace Neuron.Core.Dependencies;

// ReSharper disable once UnusedAutoPropertyAccessor.Global
public class DependencyResult<T>
{
    public List<object> Dependables { get; set; }
    public List<T> Dependencies { get; set; }
    public List<T> Solved { get; set; }
    public List<T> Unsolved { get; set; }
    public bool Successful { get; set; }
}