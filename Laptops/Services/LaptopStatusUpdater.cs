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

        // 🛒 Add to cart → 1
        public bool AddToCart(int laptopId)
        {
            return UpdateLaptopStatusById(laptopId, 1);
        }

        // ❌ Delete cart item → 0
        public bool DeleteCartItem(int laptopId)
        {
            return UpdateLaptopStatusById(laptopId, 0);
        }

        // 📦 Add to orders → 2
        public bool AddToOrders(int laptopId)
        {
            return UpdateLaptopStatusById(laptopId, 2);
        }

        // 🗑️ Delete order → back to cart → 1
        public bool DeleteOrder(int laptopId)
        {
            return UpdateLaptopStatusById(laptopId, 1);
        }

        // Optional helper: Update fifth laptop specifically
        public bool UpdateFifthLaptopStatus(int newStatus)
        {
            if (_cache.TryGetValue(CacheKey, out List<LaptopViewModel>? laptops) && laptops != null && laptops.Count >= 5)
            {
                laptops[4].userLaptopStatus = newStatus;
                _cache.Set(CacheKey, laptops, TimeSpan.FromMinutes(30));
                return true;
            }
            return false;
        }
    }
}
