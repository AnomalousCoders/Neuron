using System;
using Neuron.Core.Logging.Neuron;

namespace Neuron.Core.Logging;

public static class LoggerExtension
{
    
    public static void Verbose(this ILogger logger, Exception exception) => logger.Log(LogLevel.Verbose, exception);
    public static void Debug(this ILogger logger, Exception exception) => logger.Log(LogLevel.Debug, exception);
    public static void Info(this ILogger logger, Exception exception) => logger.Log(LogLevel.Information, exception);
    public static void Warn(this ILogger logger, Exception exception) => logger.Log(LogLevel.Warning, exception);
    public static void Error(this ILogger logger, Exception exception) => logger.Log(LogLevel.Error, exception);
    public static void Fatal(this ILogger logger, Exception exception) => logger.Log(LogLevel.Fatal, exception);
    
    public static void Log(this ILogger logger, LogLevel level, Exception exception) =>
        logger.Log(level, "[Exception]", new object[]{exception}, false);
}