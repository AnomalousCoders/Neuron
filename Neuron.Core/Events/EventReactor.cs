using System;
using System.Reflection;
using Neuron.Core.Meta;

namespace Neuron.Core.Events;

public interface IEvent { }

public class EventReactor<T>: IEventReactor where T: IEvent
{
    internal event EventHandler<T> BackingEvent;

    public void Raise(T evt)
    {
        BackingEvent?.Invoke(evt);
    }

    public void Subscribe(EventHandler<T> handler)
    {
        BackingEvent += handler;
    }

    public void Unsubscribe(EventHandler<T> handler)
    {
        BackingEvent -= handler;
    }

    public Type TypeDelegate() => typeof(T);

    public void RaiseUnsafe(object obj)
    {
        Raise((T)obj);
    }

    public object SubscribeUnsafe(object obj, MethodInfo info)
    {
        var handler = ReflectionUtils.CreateDelegate<EventHandler<T>>(obj, info);
        BackingEvent += handler;
        return handler;
    }

    public void UnsubscribeUnsafe(object subscription)
    {
        BackingEvent -= subscription as EventHandler<T>;
    }
}
    
public static class VoidEventExtension
{

    public static readonly VoidEvent RecyclableEvent = new();
        
    public static void Raise(this EventReactor<VoidEvent> reactor)
    {
        reactor.Raise(RecyclableEvent);
    }
        
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