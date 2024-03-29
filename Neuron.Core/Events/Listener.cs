﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neuron.Core.Events;

/// <summary>
/// Extendable class for easily wrapping methods to event reactor delegates.
/// Can be registered via <see cref="EventManager.RegisterListener"/>
/// </summary>
public abstract class Listener
{
    private Dictionary<Type, object> _subscriptions = new();
    private EventManager _managerReference;

    internal void RegisterAll(EventManager manger)
    {
        _managerReference = manger;
        foreach (var methodInfo in GetType().GetMethods().Where(method => method.GetCustomAttribute<EventHandlerAttribute>() is not null))
        {
            var attribute = methodInfo.GetCustomAttribute<EventHandlerAttribute>();
            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 1) throw new Exception("EventHandler must have a single Event parameter");
            var eventParameter = parameters[0];
            var eventType = eventParameter.ParameterType;
            var reactor = manger.GetUnsafe(eventType);
            var subscribeUnsafe = reactor.SubscribeUnsafe(this, methodInfo, attribute.Priority);
            _subscriptions[eventType] = subscribeUnsafe;
        }
    }

    /// <summary>
    /// Unregisters all handlers of the listener.
    /// </summary>
    /// <exception cref="Exception">if the listener is not linked</exception>
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

public class EventHandlerAttribute : Attribute {
    public int Priority { get; set; } = 0;
}