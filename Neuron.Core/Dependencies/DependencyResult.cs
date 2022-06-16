using System.Collections.Generic;

namespace Neuron.Core.Dependencies;

// ReSharper disable once UnusedAutoPropertyAccessor.Global
public class DependencyResult<T>
{
    /// <summary>
    /// All present dependables.
    /// </summary>
    public List<object> Dependables { get; set; }
    
    /// <summary>
    /// All dependencies which were queried.
    /// </summary>
    public List<T> Dependencies { get; set; }
    
    /// <summary>
    /// All solved dependencies.
    /// </summary>
    public List<T> Solved { get; set; }
    
    /// <summary>
    /// All unsolved dependencies.
    /// </summary>
    public List<T> Unsolved { get; set; }
    
    /// <summary>
    /// Whether all dependencies are resolved.
    /// </summary>
    public bool Successful { get; set; }
}