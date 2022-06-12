using System;
using System.Reflection;
using NeuronCore.Meta;

namespace NeuronCore.Events
{
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

    public delegate void EventHandler<in T>(T args) where T: IEvent;
    
    public interface IEventReactor
    {
        Type TypeDelegate();
        void RaiseUnsafe(object obj);
        object SubscribeUnsafe(object obj, MethodInfo info);
        void UnsubscribeUnsafe(object subscription);
    }
}