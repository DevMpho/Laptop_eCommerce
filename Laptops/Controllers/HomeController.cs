using Laptops.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Laptops.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LaptopDetails()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UserDetails()
        {
            var user = new UserDetails
            {
                Name = "Mahlatsi Sekele",
                Email = "mahlatsi.sekele@mintgroup.net",
                PhoneNumber = "0828616927",
                PurchaseDate = new DateTime(2025, 01, 31),
                PaymentStatus = "Paid",
                PurchaseStatus = "Approved",
                OrderNumber = "NO3",
                Model = "HP Pavilion 15",
                Processor = "Intel Core i7",
                RAM = "16GB DDR4",
                Storage = "512GB SSD",
                Graphics = "Intel Iris Xe",
                OperatingSystem = "Windows 11"
            };

            return View(user); // This looks for Views/Home/LaptopDetails.cshtml
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
