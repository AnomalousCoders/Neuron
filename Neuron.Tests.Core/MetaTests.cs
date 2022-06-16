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
            
        Assert.Null(MetaType.ExclusiveAnalyze(typeof(NonMetaType)));
        Assert.NotNull(MetaType.ExclusiveAnalyze(typeof(DirectMetaType)));
        Assert.Equal(1, MetaType.ExclusiveAnalyze(typeof(DirectMetaType)).Attributes.Length);
        Assert.Equal(0, MetaType.ExclusiveAnalyze(typeof(DirectMetaType)).Properties.Length);
        Assert.NotNull(MetaType.ExclusiveAnalyze(typeof(IndirectMetaType)));
        Assert.Equal(1, MetaType.ExclusiveAnalyze(typeof(IndirectMetaType)).Attributes.Length);
        Assert.Equal(0, MetaType.ExclusiveAnalyze(typeof(IndirectMetaType)).Properties.Length);
        Assert.NotNull(MetaType.ExclusiveAnalyze(typeof(MetaMethodType)));
        Assert.Equal(0, MetaType.ExclusiveAnalyze(typeof(MetaMethodType)).Attributes.Length);
        Assert.Equal(0, MetaType.ExclusiveAnalyze(typeof(MetaMethodType)).Properties.Length);
        Assert.NotNull(MetaType.ExclusiveAnalyze(typeof(MetaPropertyType)));
        Assert.Equal(0, MetaType.ExclusiveAnalyze(typeof(MetaPropertyType)).Attributes.Length);
        Assert.Equal(1, MetaType.ExclusiveAnalyze(typeof(MetaPropertyType)).Properties.Length);
            
        Assert.NotNull(MetaType.ExclusiveAnalyze(typeof(ExtendingMetaType)));
        Assert.Equal(1, MetaType.ExclusiveAnalyze(typeof(ExtendingMetaType)).Attributes.Length);
        Assert.NotNull(MetaType.ExclusiveAnalyze(typeof(ImplementingMetaType)));
        Assert.Equal(1, MetaType.ExclusiveAnalyze(typeof(ImplementingMetaType)).Attributes.Length);
        Assert.NotNull(MetaType.ExclusiveAnalyze(typeof(HighlyNestedMetaType)));
        Assert.Equal(1, MetaType.ExclusiveAnalyze(typeof(HighlyNestedMetaType)).Attributes.Length);
        Assert.NotNull(MetaType.ExclusiveAnalyze(typeof(OverloadMethodType)));
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