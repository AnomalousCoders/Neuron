using Neuron.Core.Logging.Neuron;

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
    
    public void Log(LogLevel level, string template, object[] args, bool isPure);

}