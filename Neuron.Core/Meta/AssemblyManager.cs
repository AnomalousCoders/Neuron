using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neuron.Core.Logging;

namespace Neuron.Core.Meta;

/// <summary>
/// Neuron service for loading assemblies.
/// </summary>
public class AssemblyManager
{
    private NeuronLogger _neuronLogger;
    private ILogger _logger;

    public AssemblyManager(NeuronLogger neuronLogger)
    {
        _neuronLogger = neuronLogger;
        _logger = _neuronLogger.GetLogger<AssemblyManager>();
    }

    private List<Assembly> _loadedAssemblies = new();

    public void SetupManager()
    {
        AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        _logger.Debug("Hooked ResolveAssembly() into current AppDomain");
    }

    /// <summary>
    /// Checks if an assembly is loaded and accessible in the current <see cref="AppDomain"/>
    /// </summary>
    /// <param name="name">The simple name of the assembly</param>
    /// <example>name: Neuron.Core</example>
    public bool IsAssemblyLoaded(string name) => AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == name);

    // Refer to Stackoverflow Question https://stackoverflow.com/a/2493855 for why we do this
    private Assembly ResolveAssembly(object sender, ResolveEventArgs args) => _loadedAssemblies.FirstOrDefault(x => x.FullName == args.Name);

    /// <summary>
    /// Loads an assembly using its raw bytes.
    /// </summary>
    /// <returns>the loaded assembly</returns>
    public Assembly LoadAssembly(byte[] bytes)
    {
        var assembly = AppDomain.CurrentDomain.Load(bytes);
        _loadedAssemblies.Add(assembly);
        _logger.Debug("Loaded assembly [Assembly]", assembly.FullName);
        return assembly;
    }
}