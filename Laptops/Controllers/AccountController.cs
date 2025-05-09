using Laptops.Data;
using Laptops.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Laptops.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Please enter a valid email.";
                return View();
            }

            // Perform case-insensitive comparison using ToLower()
            var user = _context.Employees
                .Where(u => !string.IsNullOrEmpty(u.Email) && u.Email.ToLower() == email.ToLower())
                .Select(u => u.Email)
                .FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "User not found.";
                return View();
            }

            // Redirect based on email domain
            if (email.EndsWith("@mintgroupmasp.net", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Email"] = user; // user is already a string (email)
                return RedirectToAction("MspLogin");
            }

            // All other users go to Home
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult MspLogin()
        {
            ViewBag.Email = TempData["Email"];
            return View();
        }

        [HttpPost]
        public IActionResult MspLogin(string email, string choice)
        {
            if (choice == "msp")
            {
                return RedirectToAction("MSPOrders", "Home");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
