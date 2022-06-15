using System;

namespace Neuron.Core.Logging;

public class LogToken
{
    public string Type { get; set; } = "Text";
    public LogStyle Style { get; set; } = new(ConsoleColor.White, ConsoleColor.White);
    public string Message { get; set; } = "";
}