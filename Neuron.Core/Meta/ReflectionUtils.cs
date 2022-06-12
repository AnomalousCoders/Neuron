using System;
using System.Reflection;

namespace Neuron.Core.Meta
{
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
    }
}