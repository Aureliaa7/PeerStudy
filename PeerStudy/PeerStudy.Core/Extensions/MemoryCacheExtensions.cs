using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeerStudy.Core.Extensions
{
    public static class MemoryCacheExtensions
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public static async Task RemoveRangeAsync(this IMemoryCache cache, List<string> keys)
        {
            try
            {
                await semaphore.WaitAsync();
                foreach (var key in keys)
                {
                    cache.Remove(key);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
