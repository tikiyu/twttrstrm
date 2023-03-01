using System.Collections.Concurrent;
using Twitter.Stats.Application.Common.Interfaces;

namespace Twitter.Stats.Infrastructure.Cache
{
    public class ThreadSafeMemoryCache<TKey, TValue> : IThreadSafeMemoryCache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _cache;
        private readonly ReaderWriterLockSlim _lock;

        public ThreadSafeMemoryCache()
        {
            _cache = new ConcurrentDictionary<TKey, TValue>();
            _lock = new ReaderWriterLockSlim();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            _lock.EnterReadLock();
            try
            {
                return _cache.TryGetValue(key, out value);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            _lock.EnterWriteLock();
            try
            {
                return _cache.GetOrAdd(key, valueFactory);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Set(TKey key, TValue value)
        {
            _lock.EnterWriteLock();
            try
            {
                _cache[key] = value;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool Remove(TKey key)
        {
            _lock.EnterWriteLock();
            try
            {
                return _cache.TryRemove(key, out _);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
