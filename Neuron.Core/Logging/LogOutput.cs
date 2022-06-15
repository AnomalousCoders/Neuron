using System;
using System.Collections.Generic;
using Neuron.Core.Logging.Neuron;

namespace Neuron.Core.Logging;

public class LogOutput
{
    public LogLevel Level;
    public List<LogToken> Tokens;
    public DateTime Time;
    public string Caller;

    public LogOutput(LogLevel level, List<LogToken> tokens, DateTime time, string caller)
    {
        Level = level;
        Tokens = tokens;
        Time = time;
        Caller = caller;
    }
}