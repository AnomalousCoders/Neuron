using System;
using System.Collections.Generic;

namespace Neuron.Core.Logging.Processing;

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