using Neuron.Core.Events;
using Xunit;

namespace Neuron.Tests.Core.Events;

public class ListenerTests : IClassFixture<EventSetup>
{
    private readonly EventSetup _setup;
    
    public ListenerTests(EventSetup setup)
    {
        _setup = setup;
    }
        
    [Fact]
    public void Test()
    {
        var listener = new ExampleListener();
        const string text = "Hello World!"; 
            
        _setup.EventManager.RegisterEvent<ExampleEvent>();
        Assert.NotNull(_setup.EventManager.GetSafe<ExampleEvent>());
        _setup.EventManager.UnregisterEvent<ExampleEvent>();
        Assert.False(_setup.EventManager.Reactors.TryGetValue(typeof(ExampleEvent), out _));
        _setup.EventManager.RegisterEvent(new EventReactor<ExampleEvent>());
        _setup.EventManager.RegisterListener(listener);
        _setup.EventManager.Raise(new ExampleEvent()
        {
            Text = text
        });
        Assert.True(listener.Called);
        Assert.Equal(text, listener.ArgsText);
    }
        
    public class ExampleListener : Listener
    {
        public bool Called { get; set; }
        public string ArgsText { get; set; }
            
        [EventHandler]
        public void OnExample(ExampleEvent args)
        {
            Called = true;
            ArgsText = args.Text;
        }
    }
}