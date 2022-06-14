using System.Collections.Generic;
using System.Reflection;
using Neuron.Core.Logging;
using Serilog;

namespace Neuron.Core;

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

    public Assembly LoadAssembly(byte[] bytes)
    {
        var assembly = Assembly.Load(bytes);
        _loadedAssemblies.Add(assembly);
        _logger.Debug("Loaded assembly {Assembly}", LogBox.Of(assembly.FullName));
        return assembly;
    }
    
    public Assembly LoadAssembly(string file)
    {
        var assembly = Assembly.LoadFile(file);
        _loadedAssemblies.Add(assembly);
        _logger.Debug("Loaded assembly {Assembly}", LogBox.Of(assembly.FullName));
        return assembly;
    }
}