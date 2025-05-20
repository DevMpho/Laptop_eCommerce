using Laptops.Data;
using Laptops.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Laptops.Helpers
{
    public class LaptopStatusHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LaptopStatusHelper> _logger;
        private readonly IMemoryCache _cache;

        public LaptopStatusHelper(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<LaptopStatusHelper> logger, IMemoryCache cache)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _cache = cache;
        }
        private string GetCacheKey(int employeeId) => $"UserOrders_{employeeId}";
        /// <summary>
        /// Returns a map of LaptopId to userLaptopStatus:
        /// 0 = Available (not listed in the dictionary)
        /// 1 = In Cart by current user
        /// 2 = Ordered by current user
        /// 3 = Ordered by another user
        /// </summary>
        public async Task<Dictionary<int, int>> GetLaptopStatusMapAsync()
        {
            var employeeIdStr = _httpContextAccessor.HttpContext?.Session.GetString("EmployeeId");
            if (!int.TryParse(employeeIdStr, out int employeeId))
                return new Dictionary<int, int>();


            var statusMap = new Dictionary<int, int>();

            // In Cart (status_id = 0) by current user
            var inCartLaptops = await (
                from ci in _context.CartItems
                join ec in _context.EmployeeCarts on ci.employeecart_id equals ec.employeecart_id
                where ec.employee_id == employeeId && ci.status_id == 0
                select ci.laptops_id
            ).ToListAsync();

            foreach (var id in inCartLaptops)
                statusMap[id] = 1; // 1 = In Cart

            // Ordered by current user (status_id = 1)
            var orderedByCurrentUser = await (
                from ci in _context.CartItems
                join ec in _context.EmployeeCarts on ci.employeecart_id equals ec.employeecart_id
                where ec.employee_id == employeeId && ci.status_id == 1
                select ci.laptops_id
            ).ToListAsync();

            foreach (var id in orderedByCurrentUser)
                statusMap[id] = 2; // 2 = Ordered by current user

            // Ordered by other users (status_id = 1)
            var orderedByOthers = await (
                from ci in _context.CartItems
                join ec in _context.EmployeeCarts on ci.employeecart_id equals ec.employeecart_id
                where ec.employee_id != employeeId && ci.status_id == 1
                select ci.laptops_id
            ).ToListAsync();

            foreach (var id in orderedByOthers)
            {
                if (!statusMap.ContainsKey(id))
                    statusMap[id] = 3; // 3 = Ordered by others
            }

            return statusMap;
        }
        public async Task<List<OrderViewModel>> GetEmployeeOrdersAsync(int employeeId)
        {
            try
            {
                string cacheKey = GetCacheKey(employeeId);

                // Check if the orders are cached
                if (_cache.TryGetValue(cacheKey, out List<OrderViewModel> cachedOrders))
                {
                    return cachedOrders;
                }

                // Fetch orders from the database
                var orders = await _context.Orders
                    .Where(o => o.employee_id == employeeId)
                    .Include(o => o.OrderStatus)
                    .OrderByDescending(o => o.order_date)
                    .ToListAsync();

                var orderViewModels = orders.Select(order => new OrderViewModel
                {
                    OrderId = order.order_id,
                    OrderDate = order.order_date,
                    TotalAmount = order.total_amount,
                    Status = order.OrderStatus.status_name,
                    Laptops = new List<LaptopViewModel>()
                }).ToList();

                // Cache the result
                _cache.Set(cacheKey, orderViewModels, TimeSpan.FromMinutes(30));

                return orderViewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching employee orders");
                return new List<OrderViewModel>();
            }
        }

        public void InvalidateEmployeeOrdersCache(int employeeId)
        {
            string cacheKey = GetCacheKey(employeeId);
            _cache.Remove(cacheKey);
        }

        public async Task<List<OrderViewModel>> GetUserOrdersAsync(List<LaptopViewModel> laptops)
        {
            string? employeeIdStr = _httpContextAccessor.HttpContext?.Session.GetString("EmployeeId");
            if (!int.TryParse(employeeIdStr, out int employeeId))
            {
                _logger.LogWarning("No valid EmployeeId in session.");
                return new List<OrderViewModel>();
            }

            var orders = await GetEmployeeOrdersAsync(employeeId);

            var orderedLaptops = laptops.Where(l => l.userLaptopStatus == 2).ToList();

            foreach (var order in orders)
            {
                var laptopsForOrder = orderedLaptops.Where(l => l.LaptopId == order.OrderId).ToList();
                order.Laptops.AddRange(laptopsForOrder);
            }

            return orders;
        }


    }
}
