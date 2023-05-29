namespace Neuron.Core.Logging;

public interface ILogger
{
    /// <summary>
    /// Logs a plain string
    /// </summary>
    public void Verbose(string message);
    
    /// <summary>
    /// Logs a message template with a multiple parameters.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Verbose(string template, params object[] args);
    
    /// <summary>
    /// Logs a plain string
    /// </summary>
    public void Debug(string message);
    
    /// <summary>
    /// Logs a message template with a multiple parameters.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Debug(string template, params object[] args);
    
    /// <summary>
    /// Logs a plain string
    /// </summary>
    public void Info(string message);
    
    /// <summary>
    /// Logs a message template with a multiple parameters.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Info(string template, params object[] args);
    
    /// <summary>
    /// Logs a plain string
    /// </summary>
    public void Warn(string message);
    
    /// <summary>
    /// Logs a message template with a multiple parameters.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Warn(string template, params object[] args);
    
    /// <summary>
    /// Logs a plain string
    /// </summary>
    public void Error(string message);
    
    /// <summary>
    /// Logs a message template with a multiple parameters.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Error(string template, params object[] args);
    
    /// <summary>
    /// Logs a plain string
    /// </summary>
    public void Fatal(string message);
    
    /// <summary>
    /// Logs a message template with a multiple parameters.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Fatal(string template, params object[] args);

    /// <summary>
    /// Logs a message template with a single parameter.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Verbose(string template, object arg0);
    
    /// <summary>
    /// Logs a message template with a single parameter.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Debug(string template, object arg0);
    
    /// <summary>
    /// Logs a message template with a single parameter.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Info(string template, object arg0);
    
    /// <summary>
    /// Logs a message template with a single parameter.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Warn(string template, object arg0);
    
    /// <summary>
    /// Logs a message template with a single parameter.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Error(string template, object arg0);
    
    /// <summary>
    /// Logs a message template with a single parameter.
    /// For details on the templating format see <see cref="Log"/>
    /// </summary>
    public void Fatal(string template, object arg0);
    
    /// <summary>
    /// Logs an object using the default object tokenizer
    /// </summary>
    public void Verbose(object obj);
    
    /// <summary>
    /// Logs an object using the default object tokenizer
    /// </summary>
    public void Debug(object obj);
    
    /// <summary>
    /// Logs an object using the default object tokenizer
    /// </summary>
    public void Info(object obj);
    
    /// <summary>
    /// Logs an object using the default object tokenizer
    /// </summary>
    public void Warn(object obj);
    
    /// <summary>
    /// Logs an object using the default object tokenizer
    /// </summary>
    public void Error(object obj);
    
    /// <summary>
    /// Logs an object using the default object tokenizer
    /// </summary>
    public void Fatal(object obj);

    /// <summary>
    /// Adds a log entry.
    /// The message template uses square brackets for variables. I.e.
    /// <code>"The value of myField is [Value]"</code>
    /// template strings can still use string interpolation. I.e.
    /// <code>$"The value of {myField.Name} is [Value]"</code>
    /// substituted template variables have to be passed using the
    /// <paramref name="args"/> object array
    /// </summary>
    /// <param name="level">the log severity</param>
    /// <param name="template">the message template</param>
    /// <param name="args">substitution parameters</param>
    /// <param name="isPure">logs the template as an unformatted string</param>
    /// <example>Template: $"The value of {myField.Name} is [Value]"</example>
    public void Log(LogLevel level, string template, object[] args, bool isPure);

}