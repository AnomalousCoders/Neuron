using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neuron.Core.Events
{
    public abstract class Listener
    {
        private Dictionary<Type, object> _subscriptions = new Dictionary<Type, object>();
        private EventManager _managerReference;

        internal void RegisterAll(EventManager manger)
        {
            _managerReference = manger;
            foreach (var methodInfo in GetType().GetMethods().Where(method => method.GetCustomAttribute<EventHandlerAttribute>() is not null))
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 1) throw new Exception("EventHandler must have a single Event parameter");
                var eventParameter = parameters[0];
                var eventType = eventParameter.ParameterType;
                var reactor = manger.GetUnsafe(eventType);
                _subscriptions[eventType] = reactor.SubscribeUnsafe(this, methodInfo);
            }
        }

        public void UnregisterAll()
        {
            if (_managerReference != null)
            {
                foreach (var subscription in _subscriptions)
                {
                    _managerReference.GetUnsafe(subscription.Key).UnsubscribeUnsafe(subscription.Value);
                }

                _subscriptions.Clear();
            }
            else
            {
                throw new Exception("No manager is reference");
            }
        }
    }

    public class EventHandlerAttribute : Attribute { }
}