using Neuron.Core.Modules;
using Ninject;

namespace Neuron.Modules.Commands;

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
    }

    public override void Disable()
    {
        Logger.Info("Disabling CommandModule");
    }
}