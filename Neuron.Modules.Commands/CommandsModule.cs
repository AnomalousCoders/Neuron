using Neuron.Core.Modules;
using Ninject;

namespace Neuron.Modules.Commands;

[Module(
    Name = "Commands",
    Description = "Neuron Commands Module"
)]
public class CommandsModule : Module
{
    [Inject]
    public CommandService Command { get; set; }
        
    public override void Load(IKernel kernel)
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