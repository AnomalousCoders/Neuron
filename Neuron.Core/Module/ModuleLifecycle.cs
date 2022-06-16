using System;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Logging.Diagnostics;

namespace Neuron.Core.Module
{
    public class ModuleLifecycle
    {
        public readonly EventReactor<VoidEvent> EnableComponents = new();
        public readonly EventReactor<VoidEvent> DisableComponents = new();

        public readonly EventReactor<VoidEvent> Enable = new();
        public readonly EventReactor<VoidEvent> LateEnable = new();
        public readonly EventReactor<VoidEvent> Disable = new();
        
        
        public void EnableSignal(ILogger logger, ModuleLoadContext module)
        {
            try
            {
                EnableComponents.Raise();
            }
            catch (Exception e)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while enabling module components"),
                    DiagnosticsError.Description($"Invoking the Component Enable Events of the module {module.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}."),
                    DiagnosticsError.Hint("This exception most commonly occurs when a service throws an exception in its Enable() method")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                logger.Framework(error);
            }

            try
            {
                Enable.Raise();
            }
            catch (Exception e)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while enabling a module"),
                    DiagnosticsError.Description($"Invoking the Module Enable Events of the module {module.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}."),
                    DiagnosticsError.Hint("This exception most commonly occurs when a module throws an exception in its Enable() method")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                logger.Framework(error);
            }
        }

        public void DisableSignal(ILogger logger, ModuleLoadContext module)
        {
            try
            {
                Disable.Raise();
            }
            catch (Exception e)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while disabling a module"),
                    DiagnosticsError.Description($"Invoking the Module Disable Events of the module {module.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}."),
                    DiagnosticsError.Hint("This exception most commonly occurs when a module throws an exception in its Disable() method")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                logger.Framework(error);
            }

            try
            {
                DisableComponents.Raise();
            }
            catch (Exception e)
            {
                var error = DiagnosticsError.FromParts(
                    DiagnosticsError.Summary("An error occured while disabling module components"),
                    DiagnosticsError.Description($"Invoking the Component Disable Events of the module {module.Attribute.Name} " +
                                                 $"resulted in an exception of type '{e.GetType().Name}' at call site {e.TargetSite}."),
                    DiagnosticsError.Hint("This exception most commonly occurs when a service throws an exception in its Disable() method")
                );
                error.Exception = e;
                NeuronDiagnosticHinter.AddCommonHints(e, error);
                logger.Framework(error);
            }
        }
    }
}