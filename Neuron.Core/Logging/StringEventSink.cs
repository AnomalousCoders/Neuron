using System;
using System.Text;
using Neuron.Core.Logging.Neuron;

namespace Neuron.Core.Logging;

public class StringEventSink : ILogRender
{

    private readonly LogMessageConsumer _consumer;


    public StringEventSink(LogMessageConsumer consumer)
    {
        _consumer = consumer;
    }

    public void Render(LogOutput output)
    {
        var buffer = new StringBuilder();
        buffer.Append($"[{output.Time:hh:mm:ss} ");
        switch (output.Level)
        {
            case LogLevel.Verbose:
                buffer.Append("VER");
                break;
            case LogLevel.Debug:
                buffer.Append("DBG");
                break;
            case LogLevel.Information:
                buffer.Append("INF");
                break;
            case LogLevel.Warning:
                buffer.Append("WRN");
                break;
            case LogLevel.Error:
                buffer.Append("ERR");
                break;
            case LogLevel.Fatal:
                buffer.Append("FATAL");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        buffer.Append("] ");
        foreach (var token in output.Tokens)
        {
            buffer.Append(token.Message);
        }
        _consumer.Invoke(buffer.ToString());
    }
}

public delegate void LogMessageConsumer(string str);