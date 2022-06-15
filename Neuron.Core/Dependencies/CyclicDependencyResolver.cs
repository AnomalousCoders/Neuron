using System.Collections.Generic;
using System.Linq;

namespace Neuron.Core.Dependencies;

public class CyclicDependencyResolver<T> where T: IDependencyHolder
{
    private const int MaxDepth = 255;
    private readonly List<object> _dependables = new();
    private readonly List<T> _dependencies = new();

    public void AddDependable(object t) => _dependables.Add(t);
    public void AddDependables(IEnumerable<object> list) => _dependables.AddRange(list);
    
    public void AddDependency(T t) => _dependencies.Add(t);
    public void AddDependencies(IEnumerable<T> list) => _dependencies.AddRange(list);

    // Really ReSharper? This isn't even that complex ~HelightDev
    // ReSharper disable once CognitiveComplexity
    public DependencyResult<T> Resolve()
    {
        var temporaryDependables = _dependables.ToList();
        var unsolvedDependencies = _dependencies.ToList();
        var depth = 0;
        var ordered = new List<T>();
        while (depth < MaxDepth)
        {
            var solved = new List<T>();
            foreach (var dependency in unsolvedDependencies)
            {
                if (dependency.SatisfiedBy(temporaryDependables)) solved.Add(dependency);
            }
            foreach (var dependency in solved)
            {
                ordered.Add(dependency);
                unsolvedDependencies.Remove(dependency);
                temporaryDependables.AddRange(dependency.Publications());
            }
            if (solved.Count == 0) break;
            depth++;
        }

        return new DependencyResult<T>
        {
            Successful = unsolvedDependencies.Count == 0,
            Solved = ordered,
            Dependencies = _dependencies,
            Unsolved = unsolvedDependencies,
            Dependables = temporaryDependables
        };
    }


    #region DependencyTree

    public string BuildTree() => BuildTree(Resolve());
    
    public string BuildTree(DependencyResult<T> result)
    {
        var builder = new TreeBuilder();
        foreach (var dependency in _dependencies)
        {
            RecursiveDependencyTree(builder, result, dependency);
        }

        return builder.StringBuilder.ToString().Trim();
    }

    // ReSharper disable once CognitiveComplexity
    private static void RecursiveDependencyTree(TreeBuilder builder, DependencyResult<T> result, T dependency)
    {
        var isSatisfied = dependency.SatisfiedBy(result.Dependables);

        T desiredDependency;
        if (isSatisfied)
        {
            builder.WriteLine($" {dependency}");
            builder.Increment();
            foreach (var desired in dependency.Desired())
            {
                desiredDependency = default(T);
                foreach (var dep in result.Dependencies.Where(dep => dep.Publications().Contains(desired)))
                {
                    desiredDependency = dep;
                    break;
                }

                if (desiredDependency != null)
                    RecursiveDependencyTree(builder, result, desiredDependency);
                else
                    builder.WriteLine($" {desired}");
            }
            builder.Decrement();
        }
        else
        {
            builder.WriteLine($" {dependency} [UNSATISFIED]");
            builder.Increment();
            foreach (var desired in dependency.Desired())
            {
                var existsDesired = result.Dependables.Contains(desired);
                if (existsDesired)
                {
                    desiredDependency = default(T);
                    foreach (var dep in result.Dependencies)
                    {
                        if (!dep.Publications().Contains(desired)) continue;
                        desiredDependency = dep;
                        break;
                    }

                    if (desiredDependency != null)
                        RecursiveDependencyTree(builder, result, desiredDependency);
                    else builder.WriteLine($" {desired}");
                }
                else builder.WriteLine($" {desired} [MISSING]");
            }
            builder.Decrement();
        }
    }

    #endregion
}