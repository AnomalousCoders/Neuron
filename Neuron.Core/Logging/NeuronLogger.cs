using System;
using System.Collections.Generic;
using System.Text;
using Neuron.Core.Logging.Neuron;

namespace Neuron.Core.Logging
{
    
    
    public class NeuronLogger
    {
        private LoggerController _logger;
        private NeuronBase _neuronBase;
        
        public NeuronLogger(NeuronBase neuronBase)
        {
            _neuronBase = neuronBase;
            _logger = new LoggerController();
            _logger.MinimumLevel = _neuronBase.Configuration.Logging.LogLevel;
            if (_neuronBase.Platform.Configuration.EnableConsoleLogging)
                _logger.Renderers.Add(new ConsoleRender());
            if (_neuronBase.Platform.Configuration.ConsoleWidth != -1)
                ConsoleWrapper.WidthOverride = _neuronBase.Platform.Configuration.ConsoleWidth;
            if (_neuronBase.Platform.Configuration.LogEventSink != null)
                _logger.Renderers.Add(_neuronBase.Platform.Configuration.LogEventSink);
        }

        public ILogger GetLogger<T>() => _logger.GetLogger<T>();
        public ILogger GetLogger(object owner) => _logger.GetLogger(owner.GetType());
        
        public static ILogger For<T>() => Globals.Get<NeuronLogger>().GetLogger<T>();
        public static ILogger For(Type type) => Globals.Get<NeuronLogger>().GetLogger(type);
    }
}