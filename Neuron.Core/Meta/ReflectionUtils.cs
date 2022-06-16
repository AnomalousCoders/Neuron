using System;
using System.Collections.Generic;
using System.Reflection;

namespace Neuron.Core.Meta
{
    
    /// <summary>
    /// Utilities for working with reflections.
    /// </summary>
    public static class ReflectionUtils
    {
        public static T CreateDelegate<T>(MethodInfo info) where T : Delegate
        {
            var delegateType = typeof(T);
            var delegated = info.CreateDelegate(delegateType);
            return (T)delegated;   
        }
        
        public static T CreateDelegate<T>(object instance, MethodInfo info) where T : Delegate
        {
            var delegateType = typeof(T);
            var delegated = info.CreateDelegate(delegateType, instance);
            return (T)delegated;
        }
        
        public static IEnumerable<object> ResolveInterfaceAttributes(Type type)
        {
            var attributes = new HashSet<object>();
            foreach (var face in type.GetInterfaces())
            {
                foreach (var o in face.GetCustomAttributes(true))
                {
                    attributes.Add(o);
                }
                foreach (var o in ResolveInterfaceAttributes(face))
                {
                    attributes.Add(o);
                }
            }

            return attributes;
        }
    }
}