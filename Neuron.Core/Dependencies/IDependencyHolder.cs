using System.Collections.Generic;

namespace Neuron.Core.Dependencies;

public interface IDependencyHolder
{
    /// <summary>
    /// Checks if the dependency holder is satisfied by the given dependables.
    /// </summary>
    public bool SatisfiedBy(IEnumerable<object> dependables);
    
    /// <summary>
    /// Returns all dependables which are desired by the dependency holder.
    /// </summary>
    public IEnumerable<object> Desired();
    
    /// <summary>
    /// Returns all dependables which are available after satisfying the current dependency holder. 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<object> Publications();
}