using System;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Dependencies;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Platform;
using Neuron.Modules.Configs;
using Ninject;
using Syml;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Neuron.Tests.Configs;

public class ModuleTests
{
    private readonly ITestOutputHelper _output;
    private readonly IPlatform _neuron;

    public ModuleTests(ITestOutputHelper output)
    {
        this._output = output;
        _neuron = NeuronMinimal.DebugHook(output.WriteLine);
    }

    [Fact]
    public void TestBasicModuleSetup()
    {
        var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();
        var moduleManager = _neuron.NeuronBase.Kernel.Get<ModuleManager>();
        //moduleManager.LoadModule(new []{typeof(ModuleC)});
        moduleManager.LoadModule(new[] {typeof(ConfigsModule), typeof(TranslationService), typeof(ConfigService)});
        var context = moduleManager.LoadModule(new []{typeof(ModuleA), typeof(ConfigA), typeof(TranslationsA)});

        
        
        Assert.False(moduleManager.IsLocked);
        moduleManager.ActivateModules();
        Assert.True(moduleManager.IsLocked);

        
        moduleManager.EnableAll();
        Assert.True(ModuleA.IsValueRight);
        Assert.True(ModuleA.IsMessageRight);
        
    }
}

[Module(
    Name = "ModuleA",
    Description = "Test",
    Dependencies = new []{typeof(ConfigsModule)}
)]
public class ModuleA : Module
{

    public static bool IsValueRight = false;
    public static bool IsMessageRight = false;
     
    [Inject]
    public ConfigA Config { get; set; }
    
    [Inject]
    public TranslationsA Translation { get; set; }

    public override void Enable()
    {
        if (Config.Value == "Example") IsValueRight = true;
        if (Translation.Message == "Message {0}") IsMessageRight = true;
    }
}

[Automatic]
[DocumentSection("Module A")]
public class ConfigA : IDocumentSection
{
    public string Value { get; set; } = "Example";
}

[Automatic]
public class TranslationsA : Translations<TranslationsA>
{
    public string Message { get; set; } = "Message {0}";
}