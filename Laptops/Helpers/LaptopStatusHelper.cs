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
        /// 0 = Available (not listed)
        /// 1 = In Cart by current user
        /// 2 = Ordered by current user
        /// 3 = Ordered by another user
        /// </summary>
        public async Task<Dictionary<int, int>> GetLaptopStatusMapAsync()
        {
            var employeeIdStr = _httpContextAccessor.HttpContext?.Session.GetString("EmployeeId");
            if (!int.TryParse(employeeIdStr, out int employeeId))
                return new Dictionary<int, int>();

            string userCacheKey = $"LaptopList_{employeeId}";
            var statusMap = new Dictionary<int, int>();

            // Laptops in cart (status = 1) by current user
            var inCart = await (
                from ci in _context.CartItems
                join ec in _context.EmployeeCarts on ci.employeecart_id equals ec.employeecart_id
                where ec.employee_id == employeeId && ci.status_id == 1
                select ci.laptops_id
            ).ToListAsync();

            foreach (var id in inCart)
                statusMap[id] = 1;

            // Fetch all cart items for this user
            var cartItems = await (
                from ci in _context.CartItems
                join ec in _context.EmployeeCarts on ci.employeecart_id equals ec.employeecart_id
                where ec.employee_id == employeeId
                select ci
            ).ToListAsync();

            _cache.TryGetValue(userCacheKey, out List<LaptopViewModel>? cachedLaptops);

            foreach (var item in cartItems)
            {
                if (item.status_id == 2)
                {
                    statusMap[item.laptops_id] = 2;

                    if (cachedLaptops != null)
                    {
                        var cached = cachedLaptops.FirstOrDefault(l => l.LaptopId == item.laptops_id);

                        if (cached != null)
                        {
                            cached.OrderId = item.order_id; // This MUST happen
                        }
                    }
                }
            }


            // Laptops ordered by *other* users (status = 2)
            var orderedByOthers = await (
                from ci in _context.CartItems
                join ec in _context.EmployeeCarts on ci.employeecart_id equals ec.employeecart_id
                where ec.employee_id != employeeId && ci.status_id == 2
                select ci.laptops_id
            ).ToListAsync();

            foreach (var id in orderedByOthers)
            {
                if (!statusMap.ContainsKey(id))
                    statusMap[id] = 3;
            }

            return statusMap;
        }

        public async Task<List<OrderViewModel>> GetEmployeeOrdersAsync(int employeeId)
        {
            try
            {
                string cacheKey = GetCacheKey(employeeId);

                if (_cache.TryGetValue(cacheKey, out List<OrderViewModel> cachedOrders))
                    return cachedOrders;

                var orders = await _context.Orders
                    .Where(o => o.employee_id == employeeId)
                    .Include(o => o.OrderStatus)
                    .OrderByDescending(o => o.order_date)
                    .ToListAsync();

                var viewModels = orders.Select(order => new OrderViewModel
                {
                    OrderId = order.order_id,
                    OrderDate = order.order_date,
                    TotalAmount = order.total_amount,
                    Status = order.OrderStatus.status_name,
                    Laptops = new List<LaptopViewModel>()
                }).ToList();

                _cache.Set(cacheKey, viewModels, TimeSpan.FromMinutes(30));
                return viewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching employee orders.");
                return new List<OrderViewModel>();
            }
        }

        public void InvalidateEmployeeOrdersCache(int employeeId)
        {
            _cache.Remove(GetCacheKey(employeeId));
        }

        public async Task<List<OrderViewModel>> GetUserOrdersAsync(List<LaptopViewModel> laptops)
        {
            _logger.LogInformation("➡️ Entered GetUserOrdersAsync");

            var employeeIdStr = _httpContextAccessor.HttpContext?.Session.GetString("EmployeeId");
            if (!int.TryParse(employeeIdStr, out int employeeId))
            {
                _logger.LogWarning("⚠️ Invalid or missing EmployeeId in session.");
                return new List<OrderViewModel>();
            }

            var orders = await GetEmployeeOrdersAsync(employeeId);
            // Map order ID to laptop IDs for this employee
            var orderLaptopMap = await (
                from ci in _context.CartItems
                join ec in _context.EmployeeCarts on ci.employeecart_id equals ec.employeecart_id
                where ec.employee_id == employeeId && ci.status_id == 2 && ci.order_id != null
                select new { ci.order_id, ci.laptops_id }
            ).ToListAsync();

            var orderToLaptopIds = orderLaptopMap
                .GroupBy(x => x.order_id)
                .ToDictionary(g => g.Key!.Value, g => g.Select(x => x.laptops_id).ToList());

            // Assign laptops to orders
            foreach (var order in orders)
            {
                if (orderToLaptopIds.TryGetValue(order.OrderId, out var laptopIds))
                {
                    order.Laptops = laptops
                        .Where(l => laptopIds.Contains(l.LaptopId))
                        .ToList();
                }

                _logger.LogInformation("🧾 Order ID: {OrderId}, Date: {Date}, Status: {Status}, Laptops: {Count}",
                    order.OrderId, order.OrderDate, order.Status, order.Laptops.Count);
            }


            return orders;
        }

        //msp orders 
        public async Task<List<OrderViewModel>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderStatus)
                    .Include(o => o.Employee) 
                    .OrderBy(o => o.order_date)

                    .ToListAsync();

                // Get all related CartItems for these orders
                var orderIds = orders.Select(o => o.order_id).ToList();
                var cartItems = await _context.CartItems
                    .Where(ci => ci.status_id == 2 && ci.order_id != null && orderIds.Contains(ci.order_id.Value))
                    .ToListAsync();

                // Optionally get all laptops
                var laptopIds = cartItems.Select(ci => ci.laptops_id).Distinct().ToList();
                var laptops = await _context.Laptops
                    .Where(l => laptopIds.Contains(l.laptops_id))
                    .ToListAsync();

                var laptopViewModels = laptops.Select(l => new LaptopViewModel
                {
                    LaptopId = l.laptops_id,
                    
                    Price = l.price,
                    // Add more fields as needed
                }).ToList();

                var result = new List<OrderViewModel>();

                foreach (var order in orders)
                {
                    var viewModel = new OrderViewModel
                    {
                        OrderId = order.order_id,
                        OrderDate = order.order_date,
                        TotalAmount = order.total_amount,
                        Status = order.OrderStatus.status_name,
                        Email = order.Employee?.Email ?? "N/A",

                        // if exists
                        Laptops = new List<LaptopViewModel>()
                    };

                    var laptopsInOrder = cartItems
                        .Where(ci => ci.order_id == order.order_id)
                        .Select(ci => ci.laptops_id);

                    viewModel.Laptops = laptopViewModels
                        .Where(lvm => laptopsInOrder.Contains(lvm.LaptopId))
                        .ToList();

                    result.Add(viewModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching all orders for admin.");
                return new List<OrderViewModel>();
            }
        }

        public async Task<OrderDetailsViewModel?> GetOrderDetailsByIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderStatus)
                    .Include(o => o.Employee)
                    .FirstOrDefaultAsync(o => o.order_id == orderId);

                if (order == null)
                {
                    _logger.LogWarning("⚠️ Order with ID {OrderId} not found.", orderId);
                    return null;
                }

                // Get related CartItems
                var cartItems = await _context.CartItems
                    .Where(ci => ci.status_id == 2 && ci.order_id == orderId)
                    .ToListAsync();

                var laptopIds = cartItems.Select(ci => ci.laptops_id).Distinct().ToList();

                // Get laptops with their laptop_details
                var laptops = await _context.Laptops
                    .Where(l => laptopIds.Contains(l.laptops_id))
                    .Include(l => l.LaptopDetails) // Assuming navigation property exists
                    .ToListAsync();

                var laptopViewModels = laptops.Select(l => new LaptopViewModel
                {
                    LaptopId = l.laptops_id,
                    Price = l.price,
                    ImgUrl = l.imgUrl,
                    Brand = l.LaptopDetails?.brand,
                    Model = l.LaptopDetails?.model,
                    Storage = l.LaptopDetails?.storage,
                    Ram = l.LaptopDetails?.ram,
                    ScreenSize = l.screensize,
                    Description = l.description,
                    Color = l.color,
                    BatteryLife = l.batteryLife,
                    Role = l.LaptopDetails?.role,
                    OrderId = orderId,
                    IsInCart = true,
                    userLaptopStatus = 1
                }).ToList();

                var result = new OrderDetailsViewModel
                {
                    OrderId = order.order_id,
                    OrderDate = order.order_date,
                    TotalAmount = order.total_amount,
                    OrderStatusId = order.OrderStatus.order_status_id,
                    Status = order.OrderStatus?.status_name ?? "Unknown",
                    EmployeeName = order.Employee?.firstname ?? "N/A", 
                    Email = order.Employee?.Email ?? "N/A",
                    ContactNumber = order.Employee.contactnumber, 
                    Laptops = laptopViewModels
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching order details for Order ID {OrderId}.", orderId);
                return null;
            }
        }


    }
}
