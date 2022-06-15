using System;
using System.Collections.Generic;
using Neuron.Core.Logging.Neuron;

namespace Neuron.Core.Logging;

public class LogEvent
{
    public LogLevel Level;
    public string Template;
    public List<object> Args;
    public DateTime Time;
    public string Caller;
    public bool Pure;

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