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

public class ServiceExceptionTests
{
    private readonly ITestOutputHelper _output;
    private readonly IPlatform _neuron;

    public ServiceExceptionTests(ITestOutputHelper output)
    {
        this._output = output;
        _neuron = NeuronMinimal.DebugHook(output.WriteLine);
    }

    [Fact]
    public void TestMissingServiceDependency()
    {
        var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();
        var kernel = new StandardKernel();
        kernel.BindSimple(logger);
        var metaManager = new MetaManager(logger);
        var serviceManager = new ServiceManager(kernel, metaManager);
        var moduleManager = new ModuleManager(_neuron.NeuronBase, metaManager, logger, kernel, serviceManager);
        moduleManager.LoadModule(new []{typeof(ModuleH), typeof(ServiceH)});
        moduleManager.ActivateModules();
        Assert.True(moduleManager.IsLocked);
        moduleManager.EnableAll();
        Assert.Null(moduleManager.Get("H"));
        moduleManager.DisableAll();
    }
}

[Module(
    Name = "Module H",
    Dependencies = new Type[0]
)]
public class ModuleH : Module
{
    [Inject]
    public ServiceH ServiceH { get; set; }
}


public class ServiceH : Service
{
    [Inject]
    public Unbound Unbound { get; set; }
}