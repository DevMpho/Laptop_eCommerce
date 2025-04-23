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

        public IActionResult orders()
        {
            var orders = new List<Order>
    {
        new Order {
            Id = 1,
            LaptopName = "Lenovo Legion G15",
            Description = "Features 32GB DDR5 RAM and a 16-inch display. Priced at R3 800, it comes in silver and black and is designed for gaming and development.",
            ImageUrl = "/images/legion-g15.png",
            OrderDate = new DateTime(2025, 4, 14),
            EstimatedPickupDate = new DateTime(2025, 4, 20),
            Status = "Pending"
        },
        new Order {
            Id = 2,
            LaptopName = "Lenovo ThinkBook",
            Description = "Features 32GB DDR5 RAM and a 16-inch display. It comes in silver and black and is designed for gaming and development.",
            ImageUrl = "/images/thinkbook.png",
            OrderDate = new DateTime(2025, 4, 14),
            EstimatedPickupDate = new DateTime(2025, 5, 3),
            Status = "Approved"
        },
        new Order {
            Id = 3,
            LaptopName = "Lenovo Legion 5",
            Description = "Features 32GB DDR5 RAM and a 16-inch display. The battery expectancy is around 60%.",
            ImageUrl = "/images/legion5.png",
            OrderDate = new DateTime(2025, 4, 14),
            EstimatedPickupDate = new DateTime(2025, 5, 20),
            Status = "Cancelled"
        }
    };

            return View(orders);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
