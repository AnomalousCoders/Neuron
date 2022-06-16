using System;
using System.Collections.Generic;

namespace Neuron.Core.Logging.Processing;

public class LogOutput
{
    public LogLevel Level { get; }
    public List<LogToken> Tokens { get; }
    public DateTime Time { get; }
    public string Caller { get; }

    public LogOutput(LogLevel level, List<LogToken> tokens, DateTime time, string caller)
    {
        Level = level;
        Tokens = tokens;
        Time = time;
        Caller = caller;
    }
}