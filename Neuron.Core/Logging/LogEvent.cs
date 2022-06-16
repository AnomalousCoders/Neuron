using System;
using System.Collections.Generic;

namespace Neuron.Core.Logging;

public class LogEvent
{
    public LogLevel Level { get; }
    public string Template { get; }
    public List<object> Args { get; set; }
    public DateTime Time { get; }
    public string Caller { get; }
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