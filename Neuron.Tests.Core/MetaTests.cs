using Neuron.Core;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Platform;
using Ninject;
using Xunit;
using Xunit.Abstractions;

namespace Neuron.Tests.Core;

public class MetaTests
{
    private readonly ITestOutputHelper _output;
    private readonly IPlatform _neuron;

    public MetaTests(ITestOutputHelper output)
    {
        _output = output;
        _neuron = NeuronMinimal.DebugHook();
    }

    [Fact]
    public void MetaFilterTests()
    {
        var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();;
        var metaManager = new MetaManager(logger);
            
        Assert.Null(MetaType.TryGetMetaType(typeof(NonMetaType)));
        Assert.NotNull(MetaType.TryGetMetaType(typeof(DirectMetaType)));
        Assert.Equal(1, MetaType.TryGetMetaType(typeof(DirectMetaType)).Attributes.Length);
        Assert.Equal(0, MetaType.TryGetMetaType(typeof(DirectMetaType)).Properties.Length);
        Assert.NotNull(MetaType.TryGetMetaType(typeof(IndirectMetaType)));
        Assert.Equal(1, MetaType.TryGetMetaType(typeof(IndirectMetaType)).Attributes.Length);
        Assert.Equal(0, MetaType.TryGetMetaType(typeof(IndirectMetaType)).Properties.Length);
        Assert.NotNull(MetaType.TryGetMetaType(typeof(MetaMethodType)));
        Assert.Equal(0, MetaType.TryGetMetaType(typeof(MetaMethodType)).Attributes.Length);
        Assert.Equal(0, MetaType.TryGetMetaType(typeof(MetaMethodType)).Properties.Length);
        Assert.NotNull(MetaType.TryGetMetaType(typeof(MetaPropertyType)));
        Assert.Equal(0, MetaType.TryGetMetaType(typeof(MetaPropertyType)).Attributes.Length);
        Assert.Equal(1, MetaType.TryGetMetaType(typeof(MetaPropertyType)).Properties.Length);
            
        Assert.NotNull(MetaType.TryGetMetaType(typeof(ExtendingMetaType)));
        Assert.Equal(1, MetaType.TryGetMetaType(typeof(ExtendingMetaType)).Attributes.Length);
        Assert.NotNull(MetaType.TryGetMetaType(typeof(ImplementingMetaType)));
        Assert.Equal(1, MetaType.TryGetMetaType(typeof(ImplementingMetaType)).Attributes.Length);
        Assert.NotNull(MetaType.TryGetMetaType(typeof(HighlyNestedMetaType)));
        Assert.Equal(1, MetaType.TryGetMetaType(typeof(HighlyNestedMetaType)).Attributes.Length);
        Assert.NotNull(MetaType.TryGetMetaType(typeof(OverloadMethodType)));
    }
}

public class NonMetaType { }

[Meta]
public class DirectMetaType { }
    
public class IndirectMetaAttribute : MetaAttributeBase { }

[IndirectMeta]
public class IndirectMetaType { }
    
[IndirectMeta]
public abstract class MetaSupertype { }
    
public class ExtendingMetaType : MetaSupertype { }

public interface ISecondLevelMetaObject : IMetaObject { }
public interface IThirdLevelMetaObject : ISecondLevelMetaObject { }
    
public class ImplementingMetaType : IMetaObject { }
    
public class HighlyNestedMetaType : IThirdLevelMetaObject { }

public class MetaMethodType
{
    [Meta]
    public void MetaMethod() { }
}
    
public class MetaPropertyType
{
    [Meta]
    public string MetaProperty { get; set; }
}

public abstract class OverloadBase
{
    [Meta]
    public abstract void Test();
}
    
public class OverloadMethodType : OverloadBase
{
    public override void Test() { }
}