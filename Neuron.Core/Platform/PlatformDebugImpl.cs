using System.Threading;
using Neuron.Core.Logging;
using Neuron.Core.Scheduling;

namespace Neuron.Core.Platform;

/// <summary>
/// Platform Implementation for test environments like Unit-Tests
/// </summary>
public class PlatformDebugImpl : IPlatform
{
    public PlatformConfiguration Configuration { get; set; } = new PlatformConfiguration();
    public NeuronBase NeuronBase { get; set; }
    public LoopingCoroutineReactor CoroutineReactor = new();
    private Thread _coroutineThread;
        
    public void Load()
    {
        Configuration.FileIo = false;
        Configuration.UseGlobals = false;
        Configuration.CoroutineReactor = CoroutineReactor;
        Configuration.ConsoleWidth = 100;
        NeuronBase.Configuration.Logging.FileLogging = false;
        NeuronBase.Configuration.Logging.LogLevel = LogLevel.Debug;
    }

    public void Enable()
    {
            
    }

    public void Continue()
    {
        _coroutineThread = new Thread(CoroutineReactor.Start);
        _coroutineThread.Start(); // Start coroutine Reactor in separate Thread for debug purposes
    }

    public void Disable()
    {
        CoroutineReactor.Running = false;
    }
}