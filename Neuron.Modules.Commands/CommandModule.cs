using Neuron.Core.Module;
using Ninject;


namespace Neuron.Modules.Commands
{
    [Module(
        Name = "Command",
        Description = "Neuron Command Module"
        )]
    public class CommandModule : Module
    {
        [Inject]
        public CommandService Command { get; set; }
        
        public override void Load()
        {
            Logger.Info("Loading CommandModule");
        }

        public override void Enable()
        {
            Logger.Info("Enabling CommandModule");
            var reactor = Command.CreateCommandReactor<DefaultCommandContext>();
            reactor.Handler.RegisterCommand<ExampleCommand>();
        }

        public override void Disable()
        {
            Logger.Info("Disabling CommandModule");
        }
    }
}