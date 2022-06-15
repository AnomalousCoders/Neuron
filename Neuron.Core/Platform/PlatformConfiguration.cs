using Neuron.Core.Logging;
using Neuron.Core.Scheduling;

namespace Neuron.Core.Platform;

public class PlatformConfiguration
{
    /// <summary>
    ///  Base directory for all subdirectories like Modules/, Configs/, etc.
    /// </summary>
    public string BaseDirectory { get; set; } = "/";

    /// <summary>
    /// Disables file I/O
    /// </summary>
    public bool FileIo { get; set; } = true;

    /// <summary>
    /// Enables the override for the console encoding to UTF8
    /// </summary>
    public bool OverrideConsoleEncoding { get; set; } = true;

    /// <summary>
    /// Enables hooking to the static <see cref="Globals"/>
    /// </summary>
    public bool UseGlobals { get; set; } = true;

    /// <summary>
    /// Also writes events to this consumer instead of only the console and possibly a logfile
    /// </summary>
    public ILogRender LogEventSink { get; set; } = null;

    /// <summary>
    /// The main coroutine reactor for any GameLoop related routines
    /// </summary>
    public CoroutineReactor CoroutineReactor { get; set; } = new ActionCoroutineReactor();
}