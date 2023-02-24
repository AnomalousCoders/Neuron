using System;
using System.Collections.Generic;

namespace Neuron.Core.Logging;

public class LogEvent
{
    public LogLevel Level { get; }
    
    /// <summary>
    /// The template of the string format. Is used as the log message if <see cref="Pure"/> is true.
    /// </summary>
    public string Template { get; }
    
    /// <summary>
    /// Additional specified arguments used for the string format.
    /// </summary>
    public List<object> Args { get; set; }
    /// <summary>
    /// The timestamp of the log event.
    /// </summary>
    public DateTime Time { get; }
    
    /// <summary>
    /// The name of the class or service which emitted the log event.
    /// </summary>
    public string Caller { get; }
    
    /// <summary>
    /// Defines if this log event is unformatted.
    /// </summary>
    public bool Pure { get; }

    public LogEvent(LogLevel level, string template, List<object> args, DateTime time, string caller, bool isPure)
    {
        Level = level;
        Template = template;
        Args = args;
        Time = time;
        Caller = caller;
        Pure = isPure;
    }
}