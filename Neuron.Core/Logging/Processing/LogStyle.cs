using System;

namespace Neuron.Core.Logging.Processing;

public struct LogStyle
{
    public ConsoleColor Foreground { get; set; }
    public ConsoleColor Background { get; set; }

    public LogStyle(ConsoleColor foreground, ConsoleColor background)
    {
        Foreground = foreground;
        Background = background;
    }
}