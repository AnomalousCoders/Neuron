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

public class PluginTests
{
    private readonly ITestOutputHelper _output;
    private readonly IPlatform _neuron;

    public PluginTests(ITestOutputHelper output)
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

        kernel.BindSimple(new TestInjection()
        {
            Value = 50
        });
        
        Assert.False(PluginA.IsLoaded);
        Assert.False(PluginA.IsEnabled);
        Assert.False(PluginA.IsDisabled);
        Assert.False(kernel.GetBindings(typeof(PluginA)).Any());
        var plugin = pluginManager.LoadPlugin(new[] {typeof(PluginA)}, null);
        Assert.NotNull(plugin);
        Assert.True(PluginA.IsLoaded);
        Assert.True(PluginA.IsEnabled);
        Assert.False(PluginA.IsDisabled);
        Assert.True(kernel.GetBindings(typeof(PluginA)).Any());
        Assert.Equal(50, ((PluginA)plugin.Plugin).TestInjection.Value);
        
        pluginManager.UnloadPlugin(plugin);
        Assert.True(PluginA.IsDisabled);
        Assert.False(kernel.GetBindings(typeof(PluginA)).Any());
    }
}

public class TestInjection
{
    public int Value { get; set; }
}

[Plugin(
    Name = "Plugin A",
    Description = "Example plugin A",
    Version = "1.2.3.4"
)]
public class PluginA : Plugin
{
    public static bool IsLoaded = false;
    public static bool IsEnabled = false;
    public static bool IsDisabled = false;
    
    [Inject]
    public TestInjection TestInjection { get; set; }
    
    public override void Load()
    {
        IsLoaded = true;
        Logger.Info("Loading PluginA");
    }

    public override void Enable()
    {
        IsEnabled = true;
        Logger.Info("Enabling PluginA");
    }

    public override void Disable()
    {
        IsDisabled = true;
        Logger.Info("Disabling PluginA");
    }
}