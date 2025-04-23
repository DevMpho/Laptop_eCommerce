using Laptops.Models;
using Microsoft.AspNetCore.Mvc;

namespace Laptops.Controllers
{
    public class AccountController : Controller
    {
        // Dummy users
        private static readonly List<Employee> DummyUsers = new List<Employee>
        {
            new Employee { Email = "john.doe@mintgroup.net", Role = "employee" },
            new Employee { Email = "jane.smith@mintgroup.net", Role = "employee" },
            new Employee { Email = "sam.lee@mintgroup.net", Role = "msp" },
            new Employee { Email = "lucy.brown@mintgroup.net", Role = "msp" }
        };

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email)
        {
            var user = DummyUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                ViewBag.Error = "User not found.";
                return View();
            }

            if (user.Role == "msp")
            {
                TempData["Email"] = user.Email;
                return RedirectToAction("MspLogin");
            }

            return RedirectToAction("Index", "Home"); // For employees
        }

        // GET: /Account/MspLogin
        [HttpGet]
        public IActionResult MspLogin()
        {
            ViewBag.Email = TempData["Email"];
            return View(); // This view should have two buttons to post "choice"
        }

        // POST: /Account/MspLogin
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
