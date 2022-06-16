using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Neuron.Core.Events;
using Neuron.Core.Logging.Diagnostics;

namespace Neuron.Core.Logging.Processing;

public interface ILogFormatter
{
    public LogOutput Resolve(LogEvent logEvent);
}

public class DefaultLogFormatter : ILogFormatter
{
    public static Regex regex = new Regex("\\[([A-Za-z0-9 ]*)]");

    public EventReactor<ObjectTokenizeEvent> TokenizeEventReactor = new();

    public DefaultLogFormatter()
    {
        TokenizeEventReactor.Subscribe(StringWrapper.Tokenize);
        TokenizeEventReactor.Subscribe(ExceptionTokenizer.Tokenize);
        TokenizeEventReactor.Subscribe(DiagnosticTokenizer.Tokenize);
    }

    public IEnumerable<LogToken> RunTokenizer(object obj)
    {
        if (obj == null) return new[] {NormalStringWrapper.TokenizeHighlight("null")};
        
        var args = new ObjectTokenizeEvent();
        args.Type = obj.GetType();
        args.Value = obj;
        TokenizeEventReactor.Raise(args);
        if (args.Tokens == null) FallbackTokenizer.Tokenize(args);

        return args.Tokens;
    }

    public LogOutput Resolve(LogEvent logEvent)
    {
        if (logEvent.Pure)
        {
            return new LogOutput(logEvent.Level, new[]
            {
                NormalStringWrapper.Tokenize(logEvent.Template)
            }.ToList(), logEvent.Time, logEvent.Caller);
        }
        
        var tokens = new List<LogToken>();
        var substituted = regex.Replace(logEvent.Template, "\0");
        var splicedTemplate = substituted.Split('\0');
        for (var i = 0; i < splicedTemplate.Length - 1; i++)
        {
            tokens.Add(NormalStringWrapper.Tokenize(splicedTemplate[i]));
            if (i >= logEvent.Args.Count) tokens.AddRange(RunTokenizer(null));
            else tokens.AddRange(RunTokenizer(logEvent.Args[i]));
        }
        tokens.Add(NormalStringWrapper.Tokenize(splicedTemplate.Last()));
        return new LogOutput(logEvent.Level, tokens, logEvent.Time, logEvent.Caller);
    }
}

public class ObjectTokenizeEvent : IEvent
{
    public Type Type { get; set; }
    public object Value { get; set; }
    public IEnumerable<LogToken> Tokens { get; set; }
}
