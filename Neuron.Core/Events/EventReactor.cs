using System;
using System.Reflection;
using Neuron.Core.Meta;

namespace Neuron.Core.Events;

/// <summary>
/// .Net event based proxy event handler and invocator.
/// All events used by the class should have mutable public properties for their data.
/// Event subscribers should manipulate the properties of the event they receive to alter data.
/// </summary>
/// <typeparam name="T">type of the event</typeparam>
public class EventReactor<T>: IEventReactor where T: IEvent
{
    internal event EventHandler<T> BackingEvent;

    /// <summary>
    /// Invokes the multicast event system.
    /// See <see cref="EventReactor{T}"/> for details about required property attributes.
    /// </summary>
    /// <param name="evt">the event argument object</param>
    public void Raise(T evt)
    {
        BackingEvent?.Invoke(evt);
    }

    /// <summary>
    /// Subscribes a delegate to the backing event.
    /// </summary>
    /// <param name="handler">the delegate to subscribe</param>
    public void Subscribe(EventHandler<T> handler)
    {
        BackingEvent += handler;
    }

    /// <summary>
    /// Unsubscribes a delegate from the backing event
    /// </summary>
    /// <param name="handler">the delegate to unsubscribe</param>
    public void Unsubscribe(EventHandler<T> handler)
    {
        BackingEvent -= handler;
    }

    /// <summary>
    /// Returns the type of the generic <see cref="T"/>
    /// </summary>
    public Type TypeDelegate() => typeof(T);

    /// <summary>
    /// Invokes the multicast event system.
    /// See <see cref="EventReactor{T}"/> for details about required property attributes.
    /// </summary>
    /// <param name="obj">the delegate boxed as an object</param>
    public void RaiseUnsafe(object obj)
    {
        Raise((T)obj);
    }

    /// <summary>
    /// Subscribes a method to the backing event.
    /// Uses reflections to create method delegates of type <see cref="T"/>
    /// which can be subscribed normally.
    /// </summary>
    /// <param name="obj">the instance of object which method shall be hooked</param>
    /// <param name="info">the method which shall be hooked</param>
    public object SubscribeUnsafe(object obj, MethodInfo info)
    {
        var handler = ReflectionUtils.CreateDelegate<EventHandler<T>>(obj, info);
        BackingEvent += handler;
        return handler;
    }
    
    /// <summary>
    /// Unsubscribes a delegate from the backing event
    /// </summary>
    /// <param name="subscription">the delegate boxed as an object</param>
    public void UnsubscribeUnsafe(object subscription)
    {
        BackingEvent -= subscription as EventHandler<T>;
    }
}
    
public static class VoidEventExtension
{

    public static readonly VoidEvent RecyclableEvent = new();
    
    /// <summary>
    /// Invokes the multicast event system.
    /// See <see cref="EventReactor{T}"/> for details about required property attributes.
    /// </summary>
    public static void Raise(this EventReactor<VoidEvent> reactor)
    {
        reactor.Raise(RecyclableEvent);
    }

    /// <summary>
    /// Subscribes a method to the backing event.
    /// Uses reflections to create method delegates of type T
    /// which can be subscribed normally. Uses an inlined delegate
    /// to wrap the action into the matching VoidEvent delegate type.
    /// </summary>
    /// <param name="reactor">the reactor to subscribe to</param>
    /// <param name="obj">the instance of object which method shall be hooked</param>
    /// <param name="info">the method which shall be hooked</param>
    public static object SubscribeAction(this EventReactor<VoidEvent> reactor, object obj, MethodInfo info)
    {
        var action = ReflectionUtils.CreateDelegate<Action>(obj, info);
        EventHandler<VoidEvent> handler = _ => action.Invoke();
        reactor.Subscribe(handler);
        return handler;
    }

}

public delegate void EventHandler<in T>(T args) where T: IEvent;

public class VoidEvent : IEvent { }

public interface IEventReactor
{
    Type TypeDelegate();
    void RaiseUnsafe(object obj);
    object SubscribeUnsafe(object obj, MethodInfo info);
    void UnsubscribeUnsafe(object subscription);
}