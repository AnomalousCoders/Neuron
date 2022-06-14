using System;
using Neuron.Core.Events;

namespace Neuron.Tests.Core.Events
{
    public class EventSetup : IDisposable
    {
        public EventManager EventManager { get; }

        public EventSetup()
        {
            EventManager = new EventManager();
        }

        public void Dispose()
        {
        }
    }
}

public class ExampleEvent : IEvent
{
    public string Text { get; set; }
}