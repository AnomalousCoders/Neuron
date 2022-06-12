using System;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace Neuron.Core.Logging
{
    public class NeuronLogger
    {
        private Logger _logger;
        private NeuronBase _neuronBase;
        
        public NeuronLogger(NeuronBase neuronBase)
        {
            _neuronBase = neuronBase;
            if (neuronBase.Configuration.Logging.FileLogging)
            {
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Is(neuronBase.Configuration.Logging.LogLevel)
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .WriteTo.File(neuronBase.Configuration.Logging.LogFile, rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromSeconds(5))
                    .CreateLogger();
            }
            else
            {
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .CreateLogger();
            }
        }
        
        public ILogger GetLogger<T>() => _logger.ForContext<T>();

        public ILogger GetLogger(object owner) => _logger.ForContext(owner.GetType());
        public static ILogger For<T>() => Neuron.Get<NeuronLogger>().GetLogger<T>();
        public static ILogger For(Type type) => Neuron.Get<NeuronLogger>().GetLogger(type);
    }
}