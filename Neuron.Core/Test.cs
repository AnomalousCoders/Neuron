using Neuron.Core.Events;
using Neuron.Core.Logging;
using Ninject;
using Serilog;

namespace Neuron.Core
{
    public class Test
    {
        public EventManager Manager = new EventManager();

        public void Run()
        {
            Manager.RegisterEvent<ExampleEvent>();
            Manager.RegisterListener(Neuron.Instantiate<ExampleListener>());
            Manager.Raise(new ExampleEvent()
            {
                Text = "Hello World!"
            });

            Neuron.Bind<Test2>();
        }
    }

    public class Test2
    {
        [Inject]
        public NeuronBase Base { get; set; }

        public Test2()
        {
            
        }
    }

    public class ExampleEvent : IEvent
    {
        public string Text { get; set; }
    }
    
    public class ExampleListener : Listener
    {
        private readonly ILogger _logger = NeuronLogger.For<ExampleListener>();

        [EventHandler]
        public void OnExample(ExampleEvent args)
        {
            //_logger.Information("EventHandler got {Text}", args.Text);
        }
    }
}