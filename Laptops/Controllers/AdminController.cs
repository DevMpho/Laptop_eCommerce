using Laptops.Helpers;
using Laptops.Models;
using Laptops.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Laptops.Controllers
{
    public class AdminController : Controller
    {
        private readonly LaptopStatusHelper _helper;
        private readonly ApplicationDbContext _context; // ✅ Add this line

        public AdminController(LaptopStatusHelper helper, ApplicationDbContext context)
        {
            _helper = helper;
            _context = context;  // ✅ Initialize the context from the helper
        }

        public async Task<IActionResult> Orders(string orderStatus = "All", string paymentStatus = "All", string sortBy = "All")
        {
            var orders = await _helper.GetAllOrdersAsync();

            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine("No orders found in controller.");
                return View("AllOrders", new List<OrderViewModel>());
            }

            Console.WriteLine($"Orders count before filtering: {orders.Count}");

            // Apply filters
            var filteredOrders = ApplyFilters(orders, orderStatus, paymentStatus, sortBy);

            Console.WriteLine($"Orders count after filtering: {filteredOrders.Count}");

            // Store current filter values in ViewBag for maintaining selected values
            ViewBag.CurrentOrderStatus = orderStatus;
            ViewBag.CurrentPaymentStatus = paymentStatus;
            ViewBag.CurrentSortBy = sortBy;

            return View("AllOrders", filteredOrders);
        }

        private List<OrderViewModel> ApplyFilters(List<OrderViewModel> orders, string orderStatus, string paymentStatus, string sortBy)
        {
            var filteredOrders = orders.AsQueryable();

            // Filter by Order Status
            if (!string.IsNullOrEmpty(orderStatus) && orderStatus != "All")
            {
                filteredOrders = filteredOrders.Where(o => o.Status.Equals(orderStatus, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by Payment Status
            if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
            {
                if (paymentStatus == "Paid")
                {
                    filteredOrders = filteredOrders.Where(o => !string.IsNullOrEmpty(o.PaymentStatus) && o.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase));
                }
                else if (paymentStatus == "Not Paid")
                {
                    filteredOrders = filteredOrders.Where(o => string.IsNullOrEmpty(o.PaymentStatus) || !o.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase));
                }
            }

            // Sort by Date
            if (!string.IsNullOrEmpty(sortBy) && sortBy != "All")
            {
                if (sortBy == "Newest")
                {
                    filteredOrders = filteredOrders.OrderByDescending(o => o.OrderDate);
                }
                else if (sortBy == "Oldest")
                {
                    filteredOrders = filteredOrders.OrderBy(o => o.OrderDate);
                }
            }
            else
            {
                // Default sorting by newest first
                filteredOrders = filteredOrders.OrderByDescending(o => o.OrderDate);
            }

            return filteredOrders.ToList();
        }

        public IActionResult ViewOrderDetails(int orderId)
        {
            var orderDetails = _helper.GetOrderDetailsByIdAsync(orderId).GetAwaiter().GetResult();
            if (orderDetails == null)
            {
                return NotFound();
            }
            return View(orderDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderStatus([FromBody] OrderStatusUpdateRequest request)
        {
            var order = _context.Orders.FirstOrDefault(o => o.order_id == request.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            order.order_status_id = request.NewStatusId;
            _context.SaveChanges();

            return Ok();
        }
    }
}