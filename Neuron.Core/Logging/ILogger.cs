namespace Neuron.Core.Logging;

public interface ILogger
{
    public void Verbose(string message);
    public void Verbose(string template, params object[] args);
    public void Debug(string message);
    public void Debug(string template, params object[] args);
    public void Info(string message);
    public void Info(string template, params object[] args);
    public void Warn(string message);
    public void Warn(string template, params object[] args);
    public void Error(string message);
    public void Error(string template, params object[] args);
    public void Fatal(string message);
    public void Fatal(string template, params object[] args);

    public void Verbose(string template, object arg0);
    public void Debug(string template, object arg0);
    public void Info(string template, object arg0);
    public void Warn(string template, object arg0);
    public void Error(string template, object arg0);
    public void Fatal(string template, object arg0);
    
    public void Verbose(object obj);
    public void Debug(object obj);
    public void Info(object obj);
    public void Warn(object obj);
    public void Error(object obj);
    public void Fatal(object obj);
    
    public void Log(LogLevel level, string template, object[] args, bool isPure);

}