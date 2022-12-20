using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neuron.Core.Meta;

namespace Neuron.Core.Events;

public class PrioritizedEventReactor<T> : IPrioritizedEventReactor<T> where T : IEvent
{
    internal event EventHandler<T> BackingEvent;

    internal List<PrioritizedDelegate> Delegates;

    public PrioritizedEventReactor()
    {
        Delegates = new List<PrioritizedDelegate>();
    }

    /// <inheritdoc/>
    public void Raise(T evt)
    {
        BackingEvent?.Invoke(evt);
    }

    /// <inheritdoc/>
    public void Subscribe(EventHandler<T> handler)
    {
        Subscribe(handler, 0);
    }

    /// <inheritdoc/>
    public void Subscribe(EventHandler<T> handler, int priority)
    {
        foreach (var prioritizedDelegate in Delegates)
        {
            BackingEvent -= prioritizedDelegate.Delegate;
        }
        Delegates.Add(new PrioritizedDelegate(handler, priority));
        Delegates = Delegates.OrderBy(p => p.Priority).ToList();
        foreach (var prioritizedDelegate in Delegates)
        {
            BackingEvent += prioritizedDelegate.Delegate;
        }
    }

    /// <inheritdoc/>
    public void Unsubscribe(EventHandler<T> handler)
    {
        BackingEvent -= handler;
        var prioritizedDelegate = Delegates.FirstOrDefault(p => p.Delegate == handler);
        Delegates.Remove(prioritizedDelegate);
    }

    /// <inheritdoc/>
    public Type TypeDelegate() => typeof(T);

    /// <inheritdoc/>
    public void RaiseUnsafe(object obj)
    {
        Raise((T)obj);
    }

    /// <inheritdoc/>
    public object SubscribeUnsafe(object obj, MethodInfo info)
    {
        var handler = ReflectionUtils.CreateDelegate<EventHandler<T>>(obj, info);
        BackingEvent += handler;
        return handler;
    }

    /// <inheritdoc/>
    public void UnsubscribeUnsafe(object subscription)
    {
        var casted = subscription as EventHandler<T>;
        BackingEvent -= casted;
        var prioritizedDelegate = Delegates.FirstOrDefault(p => p.Delegate == casted);
        Delegates.Remove(prioritizedDelegate);
    }

    internal class PrioritizedDelegate
    {
        public PrioritizedDelegate(EventHandler<T> @delegate, int priority) 
        { 
            Delegate = @delegate;
            Priority = priority;
        }

        public EventHandler<T> Delegate { get; set; }

        public int Priority { get; set; }
    }
}

/// <summary>
/// .Net event based proxy event handler and invocator.
/// All events used by the class should have mutable public properties for their data.
/// Event subscribers should manipulate the properties of the event they receive to alter data.
/// </summary>
/// <typeparam name="T">type of the event</typeparam>
public class EventReactor<T> : ISafeEventReactor<T> where T: IEvent
{
    internal event EventHandler<T> BackingEvent;

    /// <inheritdoc/>
    public void Raise(T ev)
    {
        BackingEvent?.Invoke(ev);
    }

    /// <inheritdoc/>
    public void Subscribe(EventHandler<T> handler)
    {
        BackingEvent += handler;
    }

    /// <inheritdoc/>
    public void Unsubscribe(EventHandler<T> handler)
    {
        BackingEvent -= handler;
    }

    /// <inheritdoc/>
    public Type TypeDelegate() => typeof(T);

    /// <inheritdoc/>
    public void RaiseUnsafe(object obj)
    {
        Raise((T)obj);
    }

    /// <inheritdoc/>
    public object SubscribeUnsafe(object obj, MethodInfo info)
    {
        var handler = ReflectionUtils.CreateDelegate<EventHandler<T>>(obj, info);
        BackingEvent += handler;
        return handler;
    }

    /// <inheritdoc/>
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

/// <summary>
/// Generique EventHandlers of type T
/// </summary>
/// <typeparam name="T">Type use for the event args</typeparam>
public delegate void EventHandler<in T>(T args) where T: IEvent;

/// <summary>
/// Empty type use to call an event without parmeters
/// </summary>
public class VoidEvent : IEvent { }

/// <summary>
/// Interface for common and prioritized EventRector 
/// </summary>
public interface IPrioritizedEventReactor<T> : ISafeEventReactor<T> where T : IEvent
{
    /// <summary>
    /// Subscribes a delegate to the backing event, 
    /// the higher the delegate's priority of execution.
    /// </summary>
    /// <param name="handler">the delegate to subscribe</param>
    /// <param name="priority">the priority of execution of the delegate</param>
    void Subscribe(EventHandler<T> handler, int priority);
}

/// <summary>
/// Interface for common and safe EventRector 
/// </summary>
public interface ISafeEventReactor<T> : IEventReactor where T : IEvent
{
    /// <summary>
    /// Invokes the multicast event system.
    /// See <see cref="EventReactor{T}"/> for details about required property attributes.
    /// </summary>
    /// <param name="ev">the event argument object</param>
    void Raise(T ev);

    /// <summary>
    /// Subscribes a delegate to the backing event.
    /// </summary>
    /// <param name="handler">the delegate to subscribe</param>
    void Subscribe(EventHandler<T> handler);

    /// <summary>
    /// Unsubscribes a delegate from the backing event.
    /// </summary>
    /// <param name="handler">the delegate to unsubscribe</param>
    void Unsubscribe(EventHandler<T> handler);
}

/// <summary>
/// Interface for common and unsafe EventRector 
/// </summary>
public interface IEventReactor
{
    /// <summary>
    /// Returns the type of the generic <see cref="T"/>.
    /// </summary>
    Type TypeDelegate();
    
    void RaiseUnsafe(object obj);

    /// <summary>
    /// Subscribes a method to the backing event.
    /// Uses reflections to create method delegates of type <see cref="T"/>
    /// which can be subscribed normally.
    /// </summary>
    /// <param name="obj">the instance of object which method shall be hooked</param>
    /// <param name="info">the method which shall be hooked</param>
    object SubscribeUnsafe(object obj, MethodInfo info);

    /// <summary>
    /// Unsubscribes a delegate from the backing event
    /// </summary>
    /// <param name="subscription">the delegate boxed as an object</param>
    void UnsubscribeUnsafe(object subscription);
}