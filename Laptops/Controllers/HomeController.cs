using Laptops.Helpers;
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
            var data = LaptopDataHelper.GetLaptopData();

            ViewBag.Featured = data.Featured;
            ViewBag.DevCreators = data.DevCreators;
            ViewBag.BusinessOffice = data.BusinessOffice;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
