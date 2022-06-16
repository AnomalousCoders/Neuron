using System;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Dependencies;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Modules;
using Neuron.Core.Platform;
using Ninject;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Neuron.Tests.Core.Modules;

public class ModuleExceptionTests
{
    private readonly ITestOutputHelper _output;
    private readonly IPlatform _neuron;

    public ModuleExceptionTests(ITestOutputHelper output)
    {
        this._output = output;
        _neuron = NeuronMinimal.DebugHook(output.WriteLine);
    }

    [Fact]
    public void TestMissingPropertyDependency()
    {
        var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();
        var kernel = new StandardKernel();
        kernel.BindSimple(logger);
        var metaManager = new MetaManager(logger);
        var serviceManager = new ServiceManager(kernel, metaManager);
        var moduleManager = new ModuleManager(_neuron.NeuronBase, metaManager, logger, kernel, serviceManager);
        moduleManager.LoadModule(new []{typeof(ModuleF)});
        moduleManager.ActivateModules();
        Assert.True(moduleManager.IsLocked);
        moduleManager.EnableAll();
        Assert.Null(moduleManager.Get("F"));
        moduleManager.DisableAll();
    }
    
    
    [Fact]
    public void TestMissingModuleDependency()
    {
        var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();
        var kernel = new StandardKernel();
        kernel.BindSimple(logger);
        var metaManager = new MetaManager(logger);
        var serviceManager = new ServiceManager(kernel, metaManager);
        var moduleManager = new ModuleManager(_neuron.NeuronBase, metaManager, logger, kernel, serviceManager);
        moduleManager.LoadModule(new []{typeof(ModuleG)});
        moduleManager.ActivateModules();
        Assert.True(moduleManager.IsLocked);
        moduleManager.EnableAll();
        Assert.Null(moduleManager.Get("G"));
        moduleManager.DisableAll();
    }
}

[Module(
    Name = "Module F",
    Dependencies = new Type[0]
)]
public class ModuleF : Module
{
    [Inject]
    public Unbound Unbound { get; set; }
}


[Module(
    Name = "Module G",
    Dependencies = new []{typeof(ModuleA)}
)]
public class ModuleG : Module { }