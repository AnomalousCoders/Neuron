using System;

namespace Neuron.Core.Logging;

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