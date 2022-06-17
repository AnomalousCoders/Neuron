using System;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Logging.Diagnostics;
using Neuron.Core.Modules;

namespace Neuron.Core.Plugins
{
    public class PluginLifecycle
    {

        private PluginContext _plugin;
        private ILogger _logger;

        public PluginLifecycle(PluginContext plugin, ILogger logger)
        {
            _plugin = plugin;
            _logger = logger;
        }

        public readonly EventReactor<VoidEvent> Enable = new();
        public readonly EventReactor<VoidEvent> Disable = new();

        public void EnableSignal()
        {
            try
            {
                Enable.Raise();
            }
            catch (Exception e)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while enabling a plugin"),
                    DiagnosticsError.Description($"Invoking the Module Enable Events of the module {_plugin.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}."),
                    DiagnosticsError.Hint("This exception most commonly occurs when a module throws an exception in its Enable() method")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                _logger.Framework(error);
            }
        }

        public void DisableSignal()
        {
            try
            {
                Disable.Raise();
            }
            catch (Exception e)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while disabling a module"),
                    DiagnosticsError.Description($"Invoking the Module Disable Events of the module {_plugin.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}."),
                    DiagnosticsError.Hint("This exception most commonly occurs when a module throws an exception in its Disable() method")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                _logger.Framework(error);
            }
        }
    }
}