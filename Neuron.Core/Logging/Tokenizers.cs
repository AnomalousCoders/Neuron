using System;

namespace Neuron.Core.Logging.Neuron;

public class StringTokenizer
{
    public static void Tokenize(ObjectTokenizeEvent args)
    {
        if (args.Value is not String) return;
        var token = new LogToken()
        {
            Message = args.Value.ToString(),
            Type = "Text",
            Style = new LogStyle(ConsoleColor.White, ConsoleColor.Black) 
        };
        args.Tokens = new[] {token};
    }
}

public class NormalStringTokenizer
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

public class ToStringTokenizer
{
    public static void Tokenize(ObjectTokenizeEvent args)
    {
        var token = new LogToken()
        {
            Message = args.Value?.ToString() ?? "null",
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