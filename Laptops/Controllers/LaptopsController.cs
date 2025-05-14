using Microsoft.AspNetCore.Mvc;
using Laptops.Models;

namespace Laptops.Controllers
{
    public class LaptopsController : Controller
    {
        private readonly LaptopService _laptopService;
        private readonly ILogger<LaptopsController> _logger;
        List<LaptopViewModel> laptops;

        public LaptopsController(LaptopService laptopService, ILogger<LaptopsController> logger)
        {
            _laptopService = laptopService;
            _logger = logger;
        }


        // Display the list of laptops
        public async Task<IActionResult> Display()
        {
            if(laptops == null)
            {
                laptops = await _laptopService.GetLaptopsAsync();
            }
            else
            {
                _logger.LogInformation("Using cached laptop data");
            }

           
            return View("Display", laptops);  // Loads Views/Laptops/Display.cshtml
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
