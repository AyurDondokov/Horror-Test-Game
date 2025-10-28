using System;
using System.Collections.Generic;

public static class GlobalEventBus
{
    static readonly Dictionary<string, Action<object>> s_events = new Dictionary<string, Action<object>>(StringComparer.Ordinal);
    static readonly Dictionary<Delegate, Action<object>> s_wrapperMap = new Dictionary<Delegate, Action<object>>();

    static readonly object s_lock = new object();

    public static void Subscribe(string eventName, Action<object> handler)
    {
        if (string.IsNullOrEmpty(eventName) || handler == null) return;
        lock (s_lock)
        {
            if (!s_events.TryGetValue(eventName, out var existing)) existing = null;
            existing += handler;
            s_events[eventName] = existing;
        }
    }

    public static void Unsubscribe(string eventName, Action<object> handler)
    {
        if (string.IsNullOrEmpty(eventName) || handler == null) return;
        lock (s_lock)
        {
            if (!s_events.TryGetValue(eventName, out var existing)) return;
            existing -= handler;
            if (existing == null) s_events.Remove(eventName);
            else s_events[eventName] = existing;
        }
    }

    public static void Invoke(string eventName, object payload = null)
    {
        if (string.IsNullOrEmpty(eventName)) return;
        Action<object> copy = null;
        lock (s_lock)
        {
            s_events.TryGetValue(eventName, out copy);
        }
        try
        {
            copy?.Invoke(payload);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
    }

    // --- Generic helpers ---

    public static void Subscribe<T>(string eventName, Action<T> callback)
    {
        if (string.IsNullOrEmpty(eventName) || callback == null) return;
        lock (s_lock)
        {
            Action<object> wrapper = (obj) =>
            {
                if (obj is T t) callback(t);
                else if (obj == null && default(T) == null)
                {
                    callback((T)obj);
                }
            };
            s_wrapperMap[callback] = wrapper;
            Subscribe(eventName, wrapper);
        }
    }

    public static void Unsubscribe<T>(string eventName, Action<T> callback)
    {
        if (string.IsNullOrEmpty(eventName) || callback == null) return;
        lock (s_lock)
        {
            if (s_wrapperMap.TryGetValue(callback, out var wrapper))
            {
                Unsubscribe(eventName, wrapper);
                s_wrapperMap.Remove(callback);
            }
        }
    }

    public static void Clear()
    {
        lock (s_lock)
        {
            s_events.Clear();
            s_wrapperMap.Clear();
        }
    }
}
