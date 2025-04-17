using Laptops.Helpers;
using Laptops.Models;
using Microsoft.AspNetCore.Mvc;

namespace Laptops.Controllers
{
    public class Laptop : Controller
    {
        public IActionResult Index()
        {
            var data = LaptopDataHelper.GetLaptopData();
            var featuredProduct = new Product2
            {
                Name = "Lenovo Legion 5",
                Description = "Gen Intel Core i5 with NVIDIA GeForce RTX 3050, 16GB DDR5 RAM, 512GB SSD",
                Price = 5000,
                ImageUrl = "/images/legion1.jpg",
                Brand = "Lenovo",
                RAM = 16       ,         
                Storage = "512GB"
            };

            var devCreators = new List<Product2>
            {
                new Product2 { Name = "Lenovo ThinkPad X1 Extreme", Description = "Intel i9, RTX 3070, 1TB SSD", Price = 5500, ImageUrl = "/images/lenovo1.jpg", Brand = "Lenovo", RAM = 32, Storage = "1TB", Memory = "15GB" },
                new Product2 { Name = "Dell XPS 15 9520", Description = "Intel i7, 16GB RAM, 1TB SSD", Price = 7300, ImageUrl = "/images/dell.jpg", Brand = "Dell", RAM = 16, Storage = "1TB", Memory = "15GB" },
                new Product2 { Name = "HP Spectre x360 2-in-1", Description = "Intel i5, 16GB RAM, 512GB SSD", Price = 6250, ImageUrl = "/images/lenovo2.jpg", Brand = "HP", RAM = 16, Storage = "512GB", Memory = "15GB" },
                new Product2 { Name = "Legion Slim 7", Description = "AMD Ryzen 7, RTX 3060, 16GB RAM, 1TB SSD", Price = 7900, ImageUrl = "/images/legion2.jpg", Brand = "Legion", RAM = 16, Storage = "1TB", Memory = "15GB" }
            };

            var businessOffice = new List<Product2>
            {
                new Product2 { Name = "Lenovo Yoga 720 2-in-1", Description = "i5, Touchscreen, 512GB SSD", Price = 4500, ImageUrl = "/images/lenovoYoga.jpg", Brand = "Lenovo", RAM = 8, Storage = "512GB", Memory = "15GB"  },
                new Product2 { Name = "ASUS Zenbook S 14", Description = "i7, 16GB RAM, 512GB SSD", Price = 5820, ImageUrl = "/images/dell2.jpg", Brand = "ASUS", RAM = 0, Storage = "512GB", Memory = "15GB" },
                new Product2 { Name = "Lenovo IdeaPad 1 15IGL7", Description = "Celeron, 4GB RAM, 256GB SSD", Price = 3200, ImageUrl = "/images/legion2.jpg", Brand = "Lenovo", RAM = 4, Storage = "256GB", Memory = "15GB" },
                new Product2 { Name = "Dell Inspiron 14", Description = "i5, 8GB RAM, 256GB SSD", Price = 4100, ImageUrl = "/images/dell3.jpg", Brand = "Dell", RAM = 8, Storage = "256GB", Memory = "15GB" }
            };

            ViewBag.Featured = featuredProduct;
            ViewBag.DevCreators = devCreators;
            ViewBag.BusinessOffice = businessOffice;

            return View();
        }
    }
}
