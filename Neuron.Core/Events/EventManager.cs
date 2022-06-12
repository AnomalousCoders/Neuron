using System;
using System.Collections.Generic;

namespace Neuron.Core.Events
{
    public class EventManager
    {
        public Dictionary<Type, IEventReactor> Reactors = new Dictionary<Type, IEventReactor>();

        public EventReactor<T> RegisterEvent<T>() where T: IEvent
        {
            var reactor = new EventReactor<T>();
            Reactors[typeof(T)] = reactor;
            return reactor;
        }

        public void RegisterEvent(IEventReactor reactor)
        {
            Reactors[reactor.TypeDelegate()] = reactor;
        }
        
        public void UnregisterEvent(Type eventType)
        {
            Reactors.Remove(eventType);
        }
        
        public void UnregisterEvent(IEventReactor reactor)
        {
            Reactors.Remove(reactor.TypeDelegate());
        }

        public void UnregisterEvent<T>()
        {
            Reactors.Remove(typeof(T));
        }
        
        public EventReactor<T> Get<T>() where T: IEvent
        {
            return (EventReactor<T>)Reactors[typeof(T)];
        }

        public IEventReactor GetUnsafe(Type type)
        {
            return Reactors[type];
        }

        public void Raise(IEvent evt)
        {
            Reactors[evt.GetType()].RaiseUnsafe(evt);
        }

        public void RegisterListener(Listener listener)
        {
            listener.RegisterAll(this);
        }
    }
}