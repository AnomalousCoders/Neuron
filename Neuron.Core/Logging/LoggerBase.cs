using System;

namespace Neuron.Core.Logging.Neuron;

public abstract class LoggerBase : ILogger
{

    public void Verbose(string message) => Log(LogLevel.Verbose, message, Array.Empty<object>(), true);
    public void Verbose(string template, params object[] args) => Log(LogLevel.Verbose, template, args);

    public void Debug(string message) => Log(LogLevel.Debug, message, Array.Empty<object>(), true);
    public void Debug(string template, params object[] args) => Log(LogLevel.Debug, template, args);

    public void Info(string message) => Log(LogLevel.Information, message, Array.Empty<object>(), true);
    public void Info(string template, params object[] args) => Log(LogLevel.Information, template, args);

    public void Warn(string message) => Log(LogLevel.Warning, message, Array.Empty<object>(), true);
    public void Warn(string template, params object[] args) => Log(LogLevel.Warning, template, args);

    public void Error(string message) => Log(LogLevel.Error, message, Array.Empty<object>(), true);
    public void Error(string template, params object[] args) => Log(LogLevel.Error, template, args);

    public void Fatal(string message) => Log(LogLevel.Fatal, message, Array.Empty<object>(), true);
    public void Fatal(string template, params object[] args) => Log(LogLevel.Fatal, template, args);
    
    public abstract void Log(LogLevel level, string template, object[] args, bool isPure = false);
}