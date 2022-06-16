using System;

namespace Neuron.Core.Logging.Processing;

public interface ILogRender
{
    void Render(LogOutput output);
}

public class ConsoleRender : ILogRender
{
    public void Render(LogOutput output)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{output.Time:hh:mm:ss} ");
        switch (output.Level)
        {
            case LogLevel.Verbose:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("VER");
                break;
            case LogLevel.Debug:
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("DBG");
                break;
            case LogLevel.Information:
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("INF");
                break;
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("WRN");
                break;
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERR");
                break;
            case LogLevel.Fatal:
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(" FATAL ");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("] ");
        
        foreach (var token in output.Tokens)
        {
            Console.BackgroundColor = token.Style.Background;
            Console.ForegroundColor = token.Style.Foreground;
            Console.Write(token.Message);
        }
        Console.WriteLine();
    }
}