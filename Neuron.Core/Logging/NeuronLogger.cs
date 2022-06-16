using System;
using System.Collections.Generic;
using System.Text;
using Neuron.Core.Logging.Processing;
using Neuron.Core.Logging.Utils;

namespace Neuron.Core.Logging
{
    /// <summary>
    /// Neurons logging service.
    /// </summary>
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

        /// <summary>
        /// Retrieves a logger which has the specified type set as its caller.
        /// </summary>
        public ILogger GetLogger<T>() 
            => _logger.GetLogger<T>();
        
        /// <summary>
        /// Retrieves a logger which has the specified type set as its caller.
        /// </summary>
        public ILogger GetLogger(Type callingType)
            => _logger.GetLogger(callingType);
        
        /// <summary>
        /// Retrieves a logger which has the specified object set as its caller.
        /// </summary>
        public ILogger GetLogger(object owner)
            => _logger.GetLogger(owner.GetType());
        
        /// <summary>
        /// Retrieves a logger using <see cref="Globals"/> which has the specified type set as its caller.
        /// </summary>
        public static ILogger For<T>()
            => Globals.Get<NeuronLogger>().GetLogger<T>();
        
        /// <summary>
        /// Retrieves a logger using <see cref="Globals"/> which has the specified type set as its caller.
        /// </summary>
        public static ILogger For(Type type) 
            => Globals.Get<NeuronLogger>().GetLogger(type);
    }
}