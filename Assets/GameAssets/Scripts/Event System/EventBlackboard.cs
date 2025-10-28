using System.Collections.Generic;

public class EventBlackboard
{
    Dictionary<string, object> data = new Dictionary<string, object>();

    public void Set<T>(string key, T value) => data[key] = value;

    public bool TryGet<T>(string key, out T value)
    {
        if (data.TryGetValue(key, out var o) && o is T t) { value = t; return true; }
        value = default; return false;
    }

    public T GetOrDefault<T>(string key, T defaultValue = default)
    {
        if (TryGet<T>(key, out var v)) return v;
        return defaultValue;
    }

    public bool Has(string key) => data.ContainsKey(key);
    public void Remove(string key) => data.Remove(key);
    public void Clear() => data.Clear();
}
