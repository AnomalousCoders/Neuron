namespace Neuron.Core.Logging;

public enum LogLevel
{
    /// <summary>
    /// Use this for output spam.
    /// Most of the time, this output is almost irrelevant.
    /// </summary>
    Verbose = 0,
    
    /// <summary>
    /// Debug information relevant to developers.
    /// Use this to log method possibly relevant
    /// method executions, objects and so on.
    /// </summary>
    Debug = 1,
    
    /// <summary>
    /// Information relevant to an normal user.
    /// Use this to inform the user about the state
    /// of the system or relevant non exceptional
    /// events.
    /// </summary>
    Information = 2,
    
    /// <summary>
    /// Low severity exceptions or warning.
    /// Use this to announce possible danger or
    /// bad practices.
    /// </summary>
    Warning = 3,
    
    /// <summary>
    /// High severity exceptions.
    /// If you see this in your console: You,
    /// and/or the developer, are doing something
    /// wrong
    /// </summary>
    Error = 4,
    
    /// <summary>
    /// Highest severity exceptions, crashes.
    /// If you see this, theres not recovery for your program,
    /// say a last farewell to your process, before it exits.
    /// </summary>
    Fatal = 5
}