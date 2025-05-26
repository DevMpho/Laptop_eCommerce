using Laptops.Helpers;
using Laptops.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Laptops.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LaptopService _laptopService;
        private readonly LaptopStatusHelper _laptopStatusHelper;


        public HomeController(ILogger<HomeController> logger, LaptopService laptopService, LaptopStatusHelper laptopStatusHelper)
        {
            _logger = logger;
            _laptopService = laptopService;
            _laptopStatusHelper = laptopStatusHelper;

        }

        public IActionResult ContactUs()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId, int laptopId)
        {
            // Remove the specific laptop from the cart/order
            await _laptopService.CancelLaptopOrderAsync(orderId, laptopId); 
            return RedirectToAction("Orders");
        }

        public async Task<IActionResult> Orders()
        {
            var employeeIdStr = HttpContext.Session.GetString("EmployeeId");

            if (int.TryParse(employeeIdStr, out int employeeId))
            {
                _laptopStatusHelper.ClearEmployeeOrderCache(employeeId);
                _logger.LogInformation("✅ Cache cleared for employee ID: {employeeId}", employeeId);
            }

            var laptops = await _laptopService.GetLaptopsAsync(); // Fetch laptops (from cache or db)
            var orders = await _laptopStatusHelper.GetUserOrdersAsync(laptops); // Get fresh orders
            return View(orders);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
