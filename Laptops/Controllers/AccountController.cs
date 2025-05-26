using Laptops.Data;
using Laptops.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace Laptops.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;


        public AccountController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        private string GetCacheKey(int employeeId)
        {
            return $"orders_{employeeId}";
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

            var user = _context.Employees
                .Where(u => u.Email.ToLower() == email.ToLower())
                .Select(u => new
                {
                    u.Email,
                    u.firstname,
                    u.lastname,
                    u.employee_id,
                    u.RoleId,
                    RoleName = u.Role.RoleName
                })
                .FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "User not found.";
                return View();
            }

            var firstInitial = !string.IsNullOrWhiteSpace(user.firstname) ? user.firstname.Substring(0, 1).ToUpper() : "";
            var lastInitial = !string.IsNullOrWhiteSpace(user.lastname) ? user.lastname.Substring(0, 1).ToUpper() : "";
            var initials = firstInitial + lastInitial;

            // Save to session
            HttpContext.Session.SetString("Initials", initials);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("EmployeeId", user.employee_id.ToString());
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
            HttpContext.Session.SetString("RoleName", user.RoleName);

            // Redirect based on role
            switch (user.RoleId)
            {
                case 1: // MSP
                    return RedirectToAction("MspLogin", "Account");
                
                default:
                    return RedirectToAction("Display", "Laptops");
            }
        }



        public IActionResult Logout()
        {
            // Get employee ID from session before clearing
            var employeeIdStr = HttpContext.Session.GetString("EmployeeId");

            if (int.TryParse(employeeIdStr, out int employeeId))
            {
                string cacheKey = GetCacheKey(employeeId);  // Use same cache key method from your orders service
                _cache.Remove(cacheKey);                      // Remove cached orders for this employee
            }

            HttpContext.Session.Clear();

            return RedirectToAction("Login");
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
                return RedirectToAction("Orders", "Admin");
            }

            return RedirectToAction("Display", "Laptops");
        }
    }
}
