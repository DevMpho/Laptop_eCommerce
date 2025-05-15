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
    }
}
