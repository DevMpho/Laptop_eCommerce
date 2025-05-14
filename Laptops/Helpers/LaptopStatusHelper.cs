using Laptops.Data;
using Microsoft.EntityFrameworkCore;


namespace Laptops.Helpers
{
    public class LaptopStatusHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LaptopStatusHelper(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /*
        /// Returns a map of LaptopId to userLaptopStatus:
        /// 2 = Requested by current user, 3 = Requested by another user
        */
        private async Task<Dictionary<int, int>> GetOrderedLaptopEmployeeMapAsync()
        {
            var userEmail = _httpContextAccessor.HttpContext?.Session.GetString("Email");

            if (string.IsNullOrEmpty(userEmail))
                return new Dictionary<int, int>();

            // Fetch all cart items with their orders and employee emails
            var result = await _context.CartItems
                .Include(ci => ci.Order)
                .ThenInclude(o => o.Employee) // ensure this navigation exists
                .Where(ci => ci.Order != null && ci.Order.Employee != null)
                .Select(ci => new
                {
                    LaptopId = ci.laptops_id,
                    EmployeeEmail = ci.Order.Employee.Email
                })
                .ToListAsync();

            var map = new Dictionary<int, int>();

            foreach (var item in result)
            {
                if (!map.ContainsKey(item.LaptopId))
                {
                    map[item.LaptopId] = item.EmployeeEmail == userEmail ? 2 : 3;
                }
            }

            return map;
        }

        // Make this public only if needed elsewhere
        public async Task<Dictionary<int, int>> GetLaptopStatusMapAsync()
        {
            return await GetOrderedLaptopEmployeeMapAsync();
        }
    }
}
