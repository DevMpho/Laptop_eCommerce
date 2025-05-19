using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Laptops.Helpers;
using Laptops.Data;
using Laptops.Models;

public class LaptopService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<LaptopService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LaptopStatusHelper _laptopStatusHelper;

    public LaptopService(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<LaptopService> logger,
        IHttpContextAccessor httpContextAccessor,
        LaptopStatusHelper laptopStatusHelper)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _laptopStatusHelper = laptopStatusHelper;
    }

    public async Task<List<LaptopViewModel>> GetLaptopsAsync()
    {
        // Get employee ID from session
        string? employeeIdStr = _httpContextAccessor.HttpContext?.Session.GetString("EmployeeId");
        if (!int.TryParse(employeeIdStr, out int employeeId))
        {
            _logger.LogWarning("Could not find valid EmployeeId in session.");
            return new List<LaptopViewModel>();
        }

        string userCacheKey = $"LaptopList_{employeeId}";

        // Check cache
        if (_cache.TryGetValue(userCacheKey, out List<LaptopViewModel> cachedLaptops))
        {
            _logger.LogInformation("✅ Cache hit for employee ID: {EmployeeId}", employeeId);
            foreach (var laptop in cachedLaptops)
            {
                _logger.LogInformation("🔁 Cached Laptop - ID: {Id}, Brand: {Brand}, Model: {Model}, Price: {Price}, Status: {Status}",
                    laptop.LaptopId, laptop.Brand, laptop.Model, laptop.Price, laptop.userLaptopStatus);
                laptop.IsInCart = _context.CartItems
                    .Any(ci => ci.employeecart_id == employeeId && ci.laptops_id == laptop.LaptopId && ci.status_id == 1);
            }

            return cachedLaptops;

        }

        try
        {
            // Fetch laptops from DB
            var laptops = await _context.Laptops
                .Include(l => l.LaptopDetails)
                .Select(l => new LaptopViewModel
                {
                    LaptopId = l.laptops_id,
                    Price = l.price,
                    ImgUrl = l.imgUrl,
                    Brand = l.LaptopDetails.brand,
                    Model = l.LaptopDetails.model,
                    Storage = l.LaptopDetails.storage,
                    Ram = l.LaptopDetails.ram,
                    ScreenSize = l.screensize,
                    Description = l.description,
                    Color = l.color,
                    Role =  l.LaptopDetails.role,
                    BatteryLife = l.batteryLife,
                    userLaptopStatus = 0, // Default status
                    IsInCart = _context.CartItems
                        .Any(ci => ci.employeecart_id == employeeId && ci.laptops_id == l.laptops_id && ci.status_id == 1)
                })
                .ToListAsync();

            _logger.LogInformation("✅ Fetched laptops from DB for employee ID: {EmployeeId}", employeeId);

            // Get status map from helper
            var statusMap = await _laptopStatusHelper.GetLaptopStatusMapAsync();

            // Assign statuses
            foreach (var laptop in laptops)
            {
                if (statusMap.TryGetValue(laptop.LaptopId, out int status))
                {
                    laptop.userLaptopStatus = status;
                }
            }

            // Cache for 5 minutes
            _cache.Set(userCacheKey, laptops, TimeSpan.FromMinutes(5));

            return laptops;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching laptops");
            return new List<LaptopViewModel>();
        }
    }
    
}
