using System;
using System.Collections.Generic;

namespace Neuron.Core.Logging.Processing;

public class StringWrapper
{
    public static void Tokenize(ObjectTokenizeEvent args)
    {
        if (args.Value is not String)
            return;
        var token = new LogToken()
        {
            Message = args.Value.ToString(),
            Type = "Text",
            Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black) 
        };
        args.Tokens = new[] {token};
    }
}

public class NormalStringWrapper
{
    public static LogToken Tokenize(string args)
    {
        return new LogToken()
        {
            Message = args,
            Type = "Text",
            Style = new LogStyle(ConsoleColor.Gray, ConsoleColor.Black) 
        };
    }
    
    public static LogToken TokenizeHighlight(string args)
    {
        return new LogToken()
        {
            Message = args,
            Type = "Text",
            Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black) 
        };
    }
}

public class FallbackTokenizer
{
    public static void Tokenize(ObjectTokenizeEvent args)
    {
        if (args.Value == null)
        {
            args.Tokens = new[]
            {
                new LogToken  { Message = "null", Type = "Normal", Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black) }
            };
            return;
        }

        string str;
        if (args.Value is IEnumerable<object> iteratable)
        {
            str = $"[{string.Join(", ", iteratable)}]";
        }
        else
        {
            str = args.Value.ToString();
        }

        var token = new LogToken()
        {
            Message = str,
            Type = "Normal",
            Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black) 
        };
        args.Tokens = new[] {token};
    }
}

public class ExceptionTokenizer
{
    public static void Tokenize(ObjectTokenizeEvent args)
    {
        if (args.Value is not Exception exception) return;
        var exceptionType = exception.GetType().FullName;
        var exceptionMessage = exception.Message;
        var exceptionTrace = exception.StackTrace;
        var header = new LogToken
        {
            Message = $"{exceptionType}: ",
            Type = "Exception",
            Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black) 
        };
        var message = new LogToken
        {
            Message = $"{exceptionMessage}\n",
            Type = "Exception",
            Style = new LogStyle(ConsoleColor.Gray, ConsoleColor.Black)
        };
        var trace = new LogToken
        {
            Message = $"{exceptionTrace}",
            Type = "Exception",
            Style = new LogStyle(ConsoleColor.DarkGray, ConsoleColor.Black)
        };
        args.Tokens = new[] {header, message, trace}; 
    }
}