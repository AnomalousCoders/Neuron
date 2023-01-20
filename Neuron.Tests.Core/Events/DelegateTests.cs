using Neuron.Core.Events;
using Xunit;

namespace Neuron.Tests.Core.Events;

public class DelegateTests : IClassFixture<EventSetup>
{
    private readonly EventSetup _setup;
    public DelegateTests(EventSetup setup)
    {
        _setup = setup;
    }
        
    public bool Called { get; set; }
    public string ArgsText { get; set; }
        
    [Fact]
    public void Test()
    {
        const string text = "Hello World!";
        _setup.EventManager.RegisterEvent<ExampleEvent>();
        Assert.NotNull(_setup.EventManager.Get<ExampleEvent>());
        Assert.True(_setup.EventManager.Reactors.TryGetValue(typeof(ExampleEvent), out _));
        _setup.EventManager.UnregisterEvent<ExampleEvent>();
        Assert.False(_setup.EventManager.Reactors.TryGetValue(typeof(ExampleEvent), out _));
        _setup.EventManager.RegisterEvent(new EventReactor<ExampleEvent>());
        _setup.EventManager.Get<ExampleEvent>().Subscribe(HandleEvent);
        _setup.EventManager.Raise(new ExampleEvent
        {
            Text = text
        });
        Assert.True(Called);
        Assert.Equal(text, ArgsText);
    }

    public void HandleEvent(ExampleEvent args)
    {
        Called = true;
        ArgsText = args.Text;
    }
}