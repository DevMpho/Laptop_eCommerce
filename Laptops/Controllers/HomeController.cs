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



        public IActionResult LaptopDetails()
        {
            return View();
            
        }
        public IActionResult ContactUs()
        {
            return View();

        }

        
        public IActionResult MSPOrders()
        {
            return View();

        }
        public IActionResult Login()
        {
            return View();
        }
        

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ShoppingCart()
        {
            var cart = new ShoppingCart
            {
                Products = new List<Product2>
                {
                    new Product2
                    {
                        Name = "Dell",
                        Description = "Intel Core i5, 8GB RAM, 512GB SSD",
                        Price = 999.99m,
                        ImageUrl = "/images/dell.jpg"
                    },
                    new Product2
                    {
                        Name = "Lenovo Legion",
                        Description = "AMD Ryzen 5, 16GB RAM, 1TB SSD",
                        Price = 1199.99m,
                        ImageUrl = "/images/legion1.jpg"
                    }
                }
            };

            return View(cart);
        }

        public IActionResult UserDetails()
        {
            /*var user = new UserDetails
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
            };*/

            return View(); // This looks for Views/Home/LaptopDetails.cshtml
        }

        public async Task<IActionResult> Orders()
        {
            var laptops = await _laptopService.GetLaptopsAsync(); // Fetch the cached laptops
            var orders = await _laptopStatusHelper.GetUserOrdersAsync(laptops); // Pass the laptops to the method
            return View(orders);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
