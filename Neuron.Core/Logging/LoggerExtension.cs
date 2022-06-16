using System;
using Neuron.Core.Logging.Diagnostics;

namespace Neuron.Core.Logging;

public static class LoggerExtension
{
    public static void Verbose(this ILogger logger, string template, object arg0) => logger.Log(LogLevel.Verbose, template, new []{arg0}, false);
    public static void Debug(this ILogger logger, string template, object arg0) => logger.Log(LogLevel.Debug, template, new []{arg0}, false);
    public static void Info(this ILogger logger, string template, object arg0) => logger.Log(LogLevel.Information,template, new []{arg0}, false);
    public static void Warn(this ILogger logger, string template, object arg0) => logger.Log(LogLevel.Warning, template, new []{arg0}, false);
    public static void Error(this ILogger logger, string template, object arg0) => logger.Log(LogLevel.Error, template, new []{arg0}, false);
    public static void Fatal(this ILogger logger, string template, object arg0) => logger.Log(LogLevel.Fatal, template, new []{arg0}, false);
    
    public static void Verbose(this ILogger logger, object obj) => logger.Log(LogLevel.Verbose, "[Obj]", new []{obj}, false);
    public static void Debug(this ILogger logger, object obj) => logger.Log(LogLevel.Debug, "[Obj]", new []{obj}, false);
    public static void Info(this ILogger logger, object obj) => logger.Log(LogLevel.Information,"[Obj]", new []{obj}, false);
    public static void Warn(this ILogger logger, object obj) => logger.Log(LogLevel.Warning, "[Obj]", new []{obj}, false);
    public static void Error(this ILogger logger, object obj) => logger.Log(LogLevel.Error, "[Obj]", new []{obj}, false);
    public static void Fatal(this ILogger logger, object obj) => logger.Log(LogLevel.Fatal, "[Obj]", new []{obj}, false);
    
    public static void Verbose(this ILogger logger, Exception exception) => logger.Log(LogLevel.Verbose, exception);
    public static void Debug(this ILogger logger, Exception exception) => logger.Log(LogLevel.Debug, exception);
    public static void Info(this ILogger logger, Exception exception) => logger.Log(LogLevel.Information, exception);
    public static void Warn(this ILogger logger, Exception exception) => logger.Log(LogLevel.Warning, exception);
    public static void Error(this ILogger logger, Exception exception) => logger.Log(LogLevel.Error, exception);
    public static void Fatal(this ILogger logger, Exception exception) => logger.Log(LogLevel.Fatal, exception);
    
    public static void Framework(this ILogger logger, DiagnosticsError error, LogLevel level) =>
        logger.Log(level, "[Diagnostic]", new[] {error}, false);
    
    public static void Framework(this ILogger logger, DiagnosticsError error) =>
        logger.Log(LogLevel.Error, "[Diagnostic]", new[] {error}, false);
    
    // Extra method since we might chance the behaviour for exceptions later
    public static void Log(this ILogger logger, LogLevel level, Exception exception) =>
        logger.Log(level, "[Exception]", new object[]{exception}, false);
}