using System;

namespace Neuron.Core.Logging;

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
    
    public void Verbose(string template, object arg0) => Log(LogLevel.Verbose, template, new []{arg0}, false);
    public void Debug(string template, object arg0) => Log(LogLevel.Debug, template, new []{arg0}, false);
    public void Info(string template, object arg0) => Log(LogLevel.Information,template, new []{arg0}, false);
    public void Warn(string template, object arg0) => Log(LogLevel.Warning, template, new []{arg0}, false);
    public void Error(string template, object arg0) => Log(LogLevel.Error, template, new []{arg0}, false);
    public void Fatal(string template, object arg0) => Log(LogLevel.Fatal, template, new []{arg0}, false);
    
    public void Verbose(object obj) => Log(LogLevel.Verbose, "[Obj]", new []{obj}, false);
    public void Debug(object obj) => Log(LogLevel.Debug, "[Obj]", new []{obj}, false);
    public void Info(object obj) => Log(LogLevel.Information,"[Obj]", new []{obj}, false);
    public void Warn(object obj) => Log(LogLevel.Warning, "[Obj]", new []{obj}, false);
    public void Error(object obj) => Log(LogLevel.Error, "[Obj]", new []{obj}, false);
    public void Fatal(object obj) => Log(LogLevel.Fatal, "[Obj]", new []{obj}, false);
    
    public abstract void Log(LogLevel level, string template, object[] args, bool isPure = false);
}