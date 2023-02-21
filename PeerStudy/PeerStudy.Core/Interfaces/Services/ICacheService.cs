using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.Services
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key, Func<Task<T>> action, MemoryCacheEntryOptions cacheEntryOptions = null);

        Task RemoveByKeysAsync(List<string> keys);
    }
}
