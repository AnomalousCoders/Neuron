using System.Collections.Generic;

namespace Neuron.Core.Dependencies;

public interface IDependencyHolder
{

    public bool SatisfiedBy(IEnumerable<object> dependables);
    public IEnumerable<object> Desired();
    public IEnumerable<object> Publications();

}