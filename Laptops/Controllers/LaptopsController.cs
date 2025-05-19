using Microsoft.AspNetCore.Mvc;
using Laptops.Models;
using Laptops.Services;

namespace Laptops.Controllers
{
    public class LaptopsController : Controller
    {
        private readonly LaptopService _laptopService;
        private readonly ILogger<LaptopsController> _logger;
        private readonly LaptopStatusUpdater _statusUpdater;
        List<LaptopViewModel> laptops;

        public LaptopsController(LaptopService laptopService, ILogger<LaptopsController> logger, LaptopStatusUpdater statusUpdater)
        {
            _laptopService = laptopService;
            _logger = logger;
            _statusUpdater = statusUpdater;
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
        [HttpPost]
        public IActionResult AddToCart([FromBody] LaptopIdRequest request)
        {
            _logger.LogInformation($"🛒 Add to cart for Laptop ID: {request.Id}");
            var result = _statusUpdater.UpdateLaptopStatusById(request.Id, 1);
            return Json(new { success = result });
        }
 

        [HttpPost]
        public IActionResult DeleteCartItem(int id)
        {
            _logger.LogInformation($"❌ Delete cart item for Laptop ID: {id}");
            var result = _statusUpdater.UpdateLaptopStatusById(id, 0); // 0 = Available
            return Json(new { success = result });
        }

        [HttpPost]
        public IActionResult AddToOrders(int id)
        {
            _logger.LogInformation($"📦 Make request (order) for Laptop ID: {id}");
            var result = _statusUpdater.UpdateLaptopStatusById(id, 2); // 2 = Ordered by current user
            return Json(new { success = result });
        }

        [HttpPost]
        public IActionResult DeleteOrder(int id)
        {
            _logger.LogInformation($"🗑️ Delete order for Laptop ID: {id}");
            var result = _statusUpdater.UpdateLaptopStatusById(id, 1); // 1 = Back to In Cart
            return Json(new { success = result });
        }


    }
}
