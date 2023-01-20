using System;
using System.Collections.Generic;

namespace Neuron.Core.Events;

public class EventManager
{
    public Dictionary<Type, IEventReactor> Reactors = new();

    /// <summary>
    /// Registers an event reactor.
    /// </summary>
    /// <typeparam name="T">type of the reactor which shall be subscribed</typeparam>
    public ISafeEventReactor<T> RegisterEvent<T>() where T: IEvent
    {
        var reactor = new EventReactor<T>();
        Reactors[typeof(T)] = reactor;
        return reactor;
    }

    /// <summary>
    /// Registers an event prioritized reactor.
    /// </summary>
    /// <typeparam name="T">type of the reactor which shall be subscribed</typeparam>
    public IPrioritizedEventReactor<T> RegisterPrioritizedEvent<T>() where T : IEvent
    {
        var reactor = new PrioritizedEventReactor<T>();
        Reactors[typeof(T)] = reactor;
        return reactor;
    }

    /// <summary>
    /// Registers an event reactor.
    /// </summary>
    public void RegisterEvent(IEventReactor reactor)
    {
        Reactors[reactor.TypeDelegate()] = reactor;
    }
     
    /// <summary>
    /// Unregisters an event reactor.
    /// </summary>
    /// <param name="eventType">type of the reactor which shall be unsubscribed</param>
    public void UnregisterEvent(Type eventType)
    {
        Reactors.Remove(eventType);
    }
    
    /// <summary>
    /// Unregisters an event reactor.
    /// </summary>
    public void UnregisterEvent(IEventReactor reactor)
    {
        Reactors.Remove(reactor.TypeDelegate());
    }

    /// <summary>
    /// Unregisters an event reactor.
    /// </summary>
    /// <typeparam name="T">type of the reactor which shall be unsubscribed</typeparam>
    public void UnregisterEvent<T>()
    {
        Reactors.Remove(typeof(T));
    }

    /// <summary>
    /// Retrieves an prioritized event reactor.
    /// </summary>
    /// <typeparam name="T">type of the reactor</typeparam>
    public IPrioritizedEventReactor<T> GetPrioritize<T>() where T : IEvent
    {
        return (IPrioritizedEventReactor<T>)Reactors[typeof(T)];
    }

    /// <summary>
    ///  Retrieves an safe event reactor.
    /// </summary>
    /// <typeparam name="T">type of the reactor</typeparam>
    public ISafeEventReactor<T> Get<T>() where T : IEvent
    {
        return (ISafeEventReactor<T>)Reactors[typeof(T)];
    }

    /// <summary>
    /// Retrieves an event reactor.
    /// </summary>
    /// <param name="type">type of the reactor</param>
    public IEventReactor GetUnsafe(Type type)
    {
        return Reactors[type];
    }
    
    /// <summary>
    /// Retrieves the reactor related to the event and invokes the multicast event system.
    /// See <see cref="EventReactor{T}"/> for details about required property attributes.
    /// </summary>
    /// <param name="evt">the event argument object</param>
    public void Raise(IEvent evt)
    {
        Reactors[evt.GetType()].RaiseUnsafe(evt);
    }

    /// <summary>
    /// Registers an event listener.
    /// </summary>
    /// <param name="listener">the listener which shall be subscribed</param>
    public void RegisterListener(Listener listener)
    {
        listener.RegisterAll(this);
    }
}