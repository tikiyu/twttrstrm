namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface IThreadSafeMemoryCache<TKey, TValue>
    {
        bool TryGetValue(TKey key, out TValue value);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);
        void Set(TKey key, TValue value);
        bool Remove(TKey key);
    }
}
