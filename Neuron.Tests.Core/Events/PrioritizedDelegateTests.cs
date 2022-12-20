using Neuron.Core.Events;
using System.Collections.Generic;
using Xunit;

namespace Neuron.Tests.Core.Events;

public class PrioritizedDelegateTests : IClassFixture<EventSetup>
{
    private readonly EventSetup _setup;
    public PrioritizedDelegateTests(EventSetup setup)
    {
        _setup = setup;
    }
        
    public List<int> Check { get; set; }
    public string ArgsText { get; set; }
        
    [Fact]
    public void Test()
    {
        const string text = "Hello World!";
        Check = new List<int>();
        var prio = _setup.EventManager.RegisterPrioritizedEvent<ExampleEvent>();
        Assert.NotNull(_setup.EventManager.GetPrioritize<ExampleEvent>());
        Assert.True(_setup.EventManager.Reactors.TryGetValue(typeof(ExampleEvent), out _));
        _setup.EventManager.UnregisterEvent<ExampleEvent>();
        Assert.False(_setup.EventManager.Reactors.TryGetValue(typeof(ExampleEvent), out _));
        _setup.EventManager.RegisterEvent(new PrioritizedEventReactor<ExampleEvent>());
        _setup.EventManager.GetPrioritize<ExampleEvent>().Subscribe(Handle0Event);
        _setup.EventManager.GetPrioritize<ExampleEvent>().Subscribe(Handle1Event, 1);
        _setup.EventManager.GetPrioritize<ExampleEvent>().Subscribe(Handle3Event, 3);
        _setup.EventManager.GetPrioritize<ExampleEvent>().Subscribe(Handle2Event, 2);
        _setup.EventManager.Raise(new ExampleEvent
        {
            Text = text
        });
        var count = Check.Count;
        Assert.True(count == 4);
        for (int i = count - 1; i >= 0; i--)
            Assert.Equal(i, Check[i]);

        Assert.Equal(text, ArgsText);
    }

    public void Handle0Event(ExampleEvent args)
    {
        Check.Add(0);
        ArgsText = args.Text;
    }

    public void Handle1Event(ExampleEvent args)
    {
        Check.Add(1);
    }

    public void Handle2Event(ExampleEvent args)
    {
        Check.Add(2);
    }

    public void Handle3Event(ExampleEvent args)
    {
        Check.Add(3);
    }
}