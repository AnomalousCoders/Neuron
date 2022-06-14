using System;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Neuron.Core.Logging;

public class StringEventSink : ILogEventSink
{

    private readonly LogMessageConsumer _consumer;


    public StringEventSink(LogMessageConsumer consumer)
    {
        _consumer = consumer;
    }

    public void Emit(LogEvent logEvent)
    {
        var str = logEvent.RenderMessage();
        _consumer.Invoke(str);
    }
}

public delegate void LogMessageConsumer(string str);