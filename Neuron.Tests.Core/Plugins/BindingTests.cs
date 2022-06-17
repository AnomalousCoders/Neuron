using System;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Platform;
using Neuron.Core.Plugins;
using Ninject;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Neuron.Tests.Core.Plugins;

public class BindingTests
{
    private readonly ITestOutputHelper _output;
    private readonly IPlatform _neuron;

    public BindingTests(ITestOutputHelper output)
    {
        this._output = output;
        _neuron = NeuronMinimal.DebugHook(output.WriteLine);
    }

    [Fact]
    public void TestBasicPluginSetup()
    {
        var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();
        var kernel = new StandardKernel();
        kernel.BindSimple(logger);

        var eventManager = new EventManager();
        var metaManager = new MetaManager(logger);
        var pluginManager = new PluginManager(_neuron.NeuronBase, kernel, metaManager, eventManager, logger);

        metaManager.MetaGenerateBindings.Subscribe(delegate(MetaGenerateBindingsEvent args)
        {
            if (args.MetaType.TryGetAttribute<BindableMetaAttribute>(out _))
            {
                args.Outputs.Add(new BindableMetaRegistration
                {
                    Type = args.MetaType.Type
                });
            }
        });

        var foundEnableRegistrations = 0;
        var foundDisableRegistrations = 0;
        
        pluginManager.PluginLoad.Subscribe(delegate(PluginLoadEvent args)
        {
            foreach (var registration in args.Context.MetaBindings.OfType<BindableMetaRegistration>())
            {
                foundEnableRegistrations += 1;
            }
        });
        
        pluginManager.PluginUnload.Subscribe(delegate(PluginUnloadEvent args)
        {
            foreach (var registration in args.Context.MetaBindings.OfType<BindableMetaRegistration>())
            {
                foundDisableRegistrations += 1;
            }
        });
        
        var plugin = pluginManager.LoadPlugin(new[] {typeof(PluginB), typeof(BoundClass)}, null);
        Assert.NotNull(plugin);
        Assert.Equal(1, foundEnableRegistrations);
        
        pluginManager.UnloadPlugin(plugin);
        Assert.Equal(1, foundDisableRegistrations);
    }
}

[Plugin(
    Name = "Plugin B",
    Description = "Example plugin B",
    Version = "1.2.3.4"
)]
public class PluginB : Plugin
{
    public override void Load()
    {
        Logger.Info("Loading PluginB");
    }

    public override void Enable()
    {
        Logger.Info("Enabling PluginB");
    }

    public override void Disable()
    {
        Logger.Info("Disabling PluginB");
    }
}

[BindableMeta]
public class BoundClass
{
    
}

public class BindableMetaAttribute : MetaAttributeBase { }

public class BindableMetaRegistration
{
    public Type Type { get; set; }
}