using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Serilog;

namespace Neuron.Core.Scheduling;

public abstract class CoroutineReactor
{
    public ILogger Logger { get; set; }
        
    private List<CoroutineRegistration> _coroutines = new();
    private ConcurrentQueue<CoroutineRegistration> _addCoroutines = new();
    private ConcurrentQueue<CoroutineRegistration> _removeCoroutines = new();

    private ReaderWriterLockSlim _rwLock = new();
        
    public object StartCoroutine(IEnumerator<float> coroutine)
    {
        var registration = new CoroutineRegistration(coroutine);
        _addCoroutines.Enqueue(registration);
        return registration;
    }
        
           
    public void StopCoroutine(object handle)
    {
        _removeCoroutines.Enqueue((CoroutineRegistration)handle);
    }
        
    protected void Tick()
    {
        while (_addCoroutines.TryDequeue(out var routine)) _coroutines.Add(routine);
        while (_removeCoroutines.TryDequeue(out var routine)) _coroutines.Remove(routine);
        var currentMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        foreach (var pair in _coroutines)
        {
            if (pair.ScheduledUpdate > currentMillis) continue;
            var coroutine = pair.Enumerator;
            try
            {
                if (coroutine.MoveNext())
                {
                    var delay = coroutine.Current;
                    pair.ScheduledUpdate = currentMillis + (long) (delay * 1000);
                }
                else
                {
                    _removeCoroutines.Enqueue(pair);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error while ticking coroutine");
            }
        }
    }
}