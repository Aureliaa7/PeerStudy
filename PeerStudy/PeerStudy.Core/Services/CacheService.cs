using Microsoft.Extensions.Caching.Memory;
using PeerStudy.Core.Extensions;
using PeerStudy.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeerStudy.Core.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;

        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private static readonly MemoryCacheEntryOptions defaultCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(900))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(1800))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(1024);

        public CacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> action, MemoryCacheEntryOptions cacheEntryOptions = null)
        {
            if (memoryCache.TryGetValue(key, out T data))
            {
                return data;
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    data = await action();
                    memoryCache.Set(key, data, cacheEntryOptions ?? defaultCacheEntryOptions);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return data;
        }

        public async Task RemoveByKeysAsync(List<string> keys)
        {
            await memoryCache.RemoveRangeAsync(keys);
        }

        public T? Get<T>(string key)
        {
            return (T?)memoryCache.Get(key);
        }
    }
}
