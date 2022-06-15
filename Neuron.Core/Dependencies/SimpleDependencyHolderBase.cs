using System.Collections.Generic;
using System.Linq;

namespace Neuron.Core.Dependencies;

public abstract class SimpleDependencyHolderBase : IDependencyHolder
{

    public virtual IEnumerable<object> Dependencies { get; }
    public virtual object Dependable { get; }

    public bool SatisfiedBy(IEnumerable<object> dependables) => Dependencies.All(dependency => dependables.Contains(dependency));
    public IEnumerable<object> Desired() => Dependencies;

    public IEnumerable<object> Publications() => new[] {Dependable};
}