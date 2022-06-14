using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Neuron.Core.Logging
{
    
    
    public class NeuronLogger
    {
        public static AnsiConsoleTheme Neuron { get; } = new(new Dictionary<ConsoleThemeStyle, string>()
        {
            [ConsoleThemeStyle.Text] = "\u001B[38;5;0253m",
            [ConsoleThemeStyle.SecondaryText] = "\u001B[38;5;0246m",
            [ConsoleThemeStyle.TertiaryText] = "\u001B[38;5;0242m",
            [ConsoleThemeStyle.Invalid] = "\u001B[33;1m",
            [ConsoleThemeStyle.Null] = "\u001B[38;5;0038m",
            [ConsoleThemeStyle.Name] = "\u001B[38;5;0081m",
            [ConsoleThemeStyle.String] = "\u001B[38;5;0216m",
            [ConsoleThemeStyle.Number] = "\u001B[38;5;151m",
            [ConsoleThemeStyle.Boolean] = "\u001B[38;5;0038m",
            [ConsoleThemeStyle.Scalar] = "\u001B[37;1m",
            [ConsoleThemeStyle.LevelVerbose] = "\u001B[37m",
            [ConsoleThemeStyle.LevelDebug] = "\u001B[37m",
            [ConsoleThemeStyle.LevelInformation] = "\u001B[37;1m",
            [ConsoleThemeStyle.LevelWarning] = "\u001B[38;5;0229m",
            [ConsoleThemeStyle.LevelError] = "\u001B[38;5;0197m\u001B[48;5;0238m",
            [ConsoleThemeStyle.LevelFatal] = "\u001B[38;5;0197m\u001B[48;5;0238m"
        });
        
        private Logger _logger;
        private NeuronBase _neuronBase;
        
        public NeuronLogger(NeuronBase neuronBase)
        {
            _neuronBase = neuronBase;
            var config = new LoggerConfiguration();
            config.Destructure.AsScalar<LogBox>();
            
            config.MinimumLevel.Is(neuronBase.Configuration.Logging.LogLevel);
            if (neuronBase.Configuration.Logging.FileLogging) config.WriteTo.Console(theme: Neuron)
                .WriteTo.File(neuronBase.Configuration.Logging.LogFile, rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromSeconds(5));
            
            else config.MinimumLevel.Is(neuronBase.Configuration.Logging.LogLevel).WriteTo
                .Console(theme: Neuron);

            if (neuronBase.Platform.Configuration.LogEventSink != null)
                config.WriteTo.Sink(neuronBase.Platform.Configuration.LogEventSink);
            
            _logger = config.CreateLogger();
        }
        
        public ILogger GetLogger<T>() => _logger.ForContext<T>();

        public ILogger GetLogger(object owner) => _logger.ForContext(owner.GetType());
        public static ILogger For<T>() => Globals.Get<NeuronLogger>().GetLogger<T>();
        public static ILogger For(Type type) => Globals.Get<NeuronLogger>().GetLogger(type);
    }
}