

namespace amorphie.shield.Extension;
public static class SignExtension
{
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key)
    {
        TValue value;
        col.TryGetValue(key, out value);
        return value;
    }
    public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key, TValue value)
    {
        TValue oldVal = col.Get(key);
        col[key] = value;
        return oldVal;
    }
}

