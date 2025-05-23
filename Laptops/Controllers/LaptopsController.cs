using Microsoft.AspNetCore.Mvc;
using Laptops.Models;
using Laptops.Services;
using Laptops.Helpers;

namespace Laptops.Controllers
{
    public class LaptopsController : Controller
    {
        private readonly LaptopService _laptopService;
        private readonly ILogger<LaptopsController> _logger;
        private readonly LaptopStatusUpdater _statusUpdater;
        private readonly LaptopStatusHelper _laptopStatusHelper;
        List<LaptopViewModel> laptops;

        public LaptopsController(LaptopService laptopService, ILogger<LaptopsController> logger, LaptopStatusUpdater statusUpdater, LaptopStatusHelper laptopStatusHelper)
        {
            _laptopService = laptopService;
            _logger = logger;
            _statusUpdater = statusUpdater;
            _laptopStatusHelper = laptopStatusHelper;
        }

        // Display the list of laptops
        public async Task<IActionResult> Display()
        {
            if (laptops == null)
            {
                laptops = await _laptopService.GetLaptopsAsync();
            }
            else
            {
                _logger.LogInformation("Using cached laptop data");
            }

            // Get the laptop status map (you might need to inject a service for this)
            var laptopStatusMap = await _laptopStatusHelper.GetLaptopStatusMapAsync();

            // Pass status map to the ViewBag so your View can access it
            ViewBag.LaptopStatusMap = laptopStatusMap;

            return View("Display", laptops);
        }


        // Show details for a single laptop (called when clicking "View Details")
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"🔍 Loading details for laptop ID: {id}");

            laptops = await _laptopService.GetLaptopsAsync();
            var laptop = laptops.FirstOrDefault(l => l.LaptopId == id);

            if (laptop == null)
            {
                _logger.LogWarning($"Laptop with ID: {id} not found.");
                return NotFound("Laptop not found");
            }

            return PartialView("_LaptopDetails", laptop);  // Returns the partial view with laptop details
        }


    }
}
