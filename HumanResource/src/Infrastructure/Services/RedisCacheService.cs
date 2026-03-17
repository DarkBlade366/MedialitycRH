using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await _cache.GetStringAsync(key, cancellationToken);
                return value == null ? default : JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache GET operation failed for key: {Key}. Returning default value.", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new DistributedCacheEntryOptions();
                if (expiry.HasValue)
                    options.SetAbsoluteExpiration(expiry.Value);

                var json = JsonSerializer.Serialize(value);
                await _cache.SetStringAsync(key, json, options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache SET operation failed for key: {Key}. Cache operation skipped.", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache REMOVE operation failed for key: {Key}. Cache operation skipped.", key);
            }
        }
    }
}