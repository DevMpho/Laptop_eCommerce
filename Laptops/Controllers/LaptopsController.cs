using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laptops.Data;
using Laptops.Models;


namespace Laptops.Controllers
{
    public class LaptopsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LaptopsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Display()
        {
            var laptops = await _context.Laptops
                .Include(l => l.LaptopDetails)
                .Select(l => new LaptopViewModel
                {
                    Price = l.price,
                    ImgUrl = l.imgUrl,
                    Brand = l.LaptopDetails.brand,
                    Model = l.LaptopDetails.model,
                    Storage = l.LaptopDetails.storage,
                    Ram = l.LaptopDetails.ram
                })
                .ToListAsync();

            return View("Display", laptops); // This loads Views/Laptops/Display.cshtml
        }
    }
}
