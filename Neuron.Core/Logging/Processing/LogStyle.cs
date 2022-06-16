using System;

namespace Neuron.Core.Logging.Processing;

public struct LogStyle
{
    public ConsoleColor Foreground;
    public ConsoleColor Background;

    public LogStyle(ConsoleColor foreground, ConsoleColor background)
    {
        
        Foreground = foreground;
        Background = background;
    }
}