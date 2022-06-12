using System;
using Neuron.Core.Events;
using Xunit;

namespace Neuron.Tests.Core
{
    public class MyTestsSetup : IDisposable
    {
        public EventManager EventManager { get; set; }

        public MyTestsSetup()
        {
            EventManager = new EventManager();
        }

        public void Dispose()
        {
        }
    }

    public class Tests : IClassFixture<MyTestsSetup>
    {
        private readonly MyTestsSetup _setup;
        public Tests(MyTestsSetup setup)
        {
            _setup = setup;
        }
        
        [Fact]
        public void Test1()
        {
            var listener = new ExampleListener();
            var text = "Hello World!"; 
            
            _setup.EventManager.RegisterEvent<ExampleEvent>();
            _setup.EventManager.RegisterListener(listener);
            _setup.EventManager.Raise(new ExampleEvent()
            {
                Text = text
            });

            Assert.True(listener.Called);
            Assert.Equal(text, listener.ArgsText);
        }
        
        public class ExampleEvent : IEvent
        {
            public string Text { get; set; }
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
}