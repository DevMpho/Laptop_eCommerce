using Laptops.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Laptops.Controllers
{
    public class AdminController : Controller
    {
        private readonly LaptopStatusHelper _helper;

        public AdminController(LaptopStatusHelper helper)
        {
            _helper = helper;
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _helper.GetAllOrdersAsync();

            if (orders == null || orders.Count == 0)
            {
                // Log or debug here
                Console.WriteLine("No orders found in controller.");
            }
            else
            {
                Console.WriteLine($"Orders count: {orders.Count}");
            }

            return View("AllOrders", orders);
        }

    }
}
