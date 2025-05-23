using Laptops.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Laptops.Services
{
    public class LaptopStatusUpdater
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<LaptopStatusUpdater> _logger;
        private const string CacheKey = "CachedLaptops";

        public LaptopStatusUpdater(IMemoryCache cache, ILogger<LaptopStatusUpdater> logger)
        {
            _cache = cache;
            _logger = logger;
            _logger.LogInformation("✅ LaptopStatusUpdater service initialized.");
        }

        public void CacheLaptops(List<LaptopViewModel> laptops)
        {
            _cache.Set(CacheKey, laptops, TimeSpan.FromMinutes(30));
        }

        public List<LaptopViewModel>? GetCachedLaptops()
        {
            _cache.TryGetValue(CacheKey, out List<LaptopViewModel>? laptops);
            return laptops;
        }

        public bool UpdateLaptopStatusById(int laptopId, int newStatus)
        {
            if (_cache.TryGetValue(CacheKey, out List<LaptopViewModel>? laptops) && laptops != null)
            {
                var laptop = laptops.FirstOrDefault(l => l.LaptopId == laptopId);
                if (laptop != null)
                {
                    laptop.userLaptopStatus = newStatus;
                    _logger.LogInformation($"🔄 Updated laptop ID {laptopId} to status {newStatus}");

                    _cache.Set(CacheKey, laptops, TimeSpan.FromMinutes(30));
                    return true;
                }
                _logger.LogWarning($"⚠️ Laptop with ID {laptopId} not found in cache.");
            }
            else
            {
                _logger.LogWarning("⚠️ No cached laptops found.");
            }

            return false;
        }

        

        
    }
}
