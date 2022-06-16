using System;
using Neuron.Core.Logging.Diagnostics;

namespace Neuron.Core.Logging;

public static class LoggerExtension
{
    
    
    /// <summary>
    /// Logs an exception.
    /// </summary>
    public static void Verbose(this ILogger logger, Exception exception) 
        => logger.Log(LogLevel.Verbose, exception);
    
    /// <summary>
    /// Logs an exception.
    /// </summary>
    public static void Debug(this ILogger logger, Exception exception) 
        => logger.Log(LogLevel.Debug, exception);
    
    /// <summary>
    /// Logs an exception.
    /// </summary>
    public static void Info(this ILogger logger, Exception exception) 
        => logger.Log(LogLevel.Information, exception);
    
    /// <summary>
    /// Logs an exception.
    /// </summary>
    public static void Warn(this ILogger logger, Exception exception) 
        => logger.Log(LogLevel.Warning, exception);
    
    /// <summary>
    /// Logs an exception.
    /// </summary>
    public static void Error(this ILogger logger, Exception exception) 
        => logger.Log(LogLevel.Error, exception);
    
    /// <summary>
    /// Logs an exception.
    /// </summary>
    public static void Fatal(this ILogger logger, Exception exception)
        => logger.Log(LogLevel.Fatal, exception);
        
    /// <summary>
    /// Logs a framework diagnostics error at the specified level.
    /// </summary>
    public static void Framework(this ILogger logger, DiagnosticsError error, LogLevel level) 
        => logger.Log(level, "[Diagnostic]", new[] {error}, false);
    
    /// <summary>
    /// Logs a framework diagnostics error at Error Level.
    /// </summary>
    public static void Framework(this ILogger logger, DiagnosticsError error) 
        => logger.Log(LogLevel.Error, "[Diagnostic]", new[] {error}, false);

    /// <summary>
    /// Logs an exception at a specified LogLevel.
    /// </summary>
    // Extra method since we might chance the behaviour for exceptions later
    public static void Log(this ILogger logger, LogLevel level, Exception exception) 
        => logger.Log(level, "[Exception]", new object[]{exception}, false);
}