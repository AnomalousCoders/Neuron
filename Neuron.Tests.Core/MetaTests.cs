using System;
using System.Linq;
using Neuron.Core;
using Neuron.Core.Events;
using Neuron.Core.Logging;
using Neuron.Core.Meta;
using Neuron.Core.Platform;
using Ninject;
using Xunit;
using Xunit.Abstractions;

namespace Neuron.Tests.Core
{
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
        public void Test()
        {
            var logger = _neuron.NeuronBase.Kernel.Get<NeuronLogger>();;
            var metaManager = new MetaManager(logger);
            
            Assert.Null(MetaType.Analyze(typeof(NonMetaType)));
            Assert.NotNull(MetaType.Analyze(typeof(DirectMetaType)));
            Assert.Equal(1, MetaType.Analyze(typeof(DirectMetaType)).Attributes.Length);
            Assert.Equal(0, MetaType.Analyze(typeof(DirectMetaType)).Properties.Length);
            Assert.NotNull(MetaType.Analyze(typeof(IndirectMetaType)));
            Assert.Equal(1, MetaType.Analyze(typeof(IndirectMetaType)).Attributes.Length);
            Assert.Equal(0, MetaType.Analyze(typeof(IndirectMetaType)).Properties.Length);
            Assert.NotNull(MetaType.Analyze(typeof(MetaMethodType)));
            Assert.Equal(0, MetaType.Analyze(typeof(MetaMethodType)).Attributes.Length);
            Assert.Equal(0, MetaType.Analyze(typeof(MetaMethodType)).Properties.Length);
            Assert.NotNull(MetaType.Analyze(typeof(MetaPropertyType)));
            Assert.Equal(0, MetaType.Analyze(typeof(MetaPropertyType)).Attributes.Length);
            Assert.Equal(1, MetaType.Analyze(typeof(MetaPropertyType)).Properties.Length);
            
            Assert.NotNull(MetaType.Analyze(typeof(ExtendingMetaType)));
            Assert.Equal(1, MetaType.Analyze(typeof(ExtendingMetaType)).Attributes.Length);
            Assert.NotNull(MetaType.Analyze(typeof(ImplementingMetaType)));
            Assert.Equal(1, MetaType.Analyze(typeof(ImplementingMetaType)).Attributes.Length);
            Assert.NotNull(MetaType.Analyze(typeof(HighlyNestedMetaType)));
            Assert.Equal(1, MetaType.Analyze(typeof(HighlyNestedMetaType)).Attributes.Length);
            Assert.NotNull(MetaType.Analyze(typeof(OverloadMethodType)));
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
}