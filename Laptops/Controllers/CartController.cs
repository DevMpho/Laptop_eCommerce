using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Laptops.Models;
using Laptops.Data;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult AddToCart(int laptopId)
    {
        var employeeIdString = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrEmpty(employeeIdString))
            return Json(new { success = false, message = "Not logged in." });

        int employeeId = int.Parse(employeeIdString);

        // Check if cart exists
        var cart = _context.EmployeeCarts.FirstOrDefault(c => c.employee_id == employeeId);
        if (cart == null)
        {
            cart = new employee_cart
            {
                employee_id = employeeId,
                created_at = DateTime.Now
            };
            _context.EmployeeCarts.Add(cart);
            _context.SaveChanges();
        }

        // Add to cart_items
        var cartItem = new cart_items
        {
            laptops_id = laptopId,
            employeecart_id = cart.employeecart_id,
            status_id = 1, 
           
        };

        _context.CartItems.Add(cartItem);
        _context.SaveChanges();

        // Count cart items
        int count = _context.CartItems.Count(ci => ci.employeecart_id == cart.employeecart_id);

        return Json(new { success = true, cartCount = count });
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        var employeeIdStr = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrEmpty(employeeIdStr))
            return Json(new { count = 0 });

        int employeeId = int.Parse(employeeIdStr);
        var cart = _context.EmployeeCarts.FirstOrDefault(c => c.employee_id == employeeId);

        int count = 0;
        if (cart != null)
        {
            count = _context.CartItems
                            .Where(ci => ci.employeecart_id == cart.employeecart_id)
                            .Count();
        }

        return Json(new { count });
    }

    public IActionResult Sidebar()
    {
        var employeeIdStr = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrEmpty(employeeIdStr)) return PartialView("_CartSidebar", new List<cart_items>());

        int employeeId = int.Parse(employeeIdStr);
        var cart = _context.EmployeeCarts.FirstOrDefault(c => c.employee_id == employeeId);
        if (cart == null) return PartialView("_CartSidebar", new List<cart_items>());

        var items = _context.CartItems
            .Where(ci => ci.employeecart_id == cart.employeecart_id)
            .Include(ci => ci.Laptop)
            .ThenInclude(l => l!.LaptopDetails)
            .ToList();

        return PartialView("_CartSidebar", items); 
    }



    [HttpPost]
    public IActionResult RemoveFromCart(int id)
    {
        var item = _context.CartItems.FirstOrDefault(ci => ci.cartitems_id == id);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            _context.SaveChanges();
        }

        // Get current employeeId from session
        var employeeIdStr = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrEmpty(employeeIdStr))
            return PartialView("_CartSidebar", new List<cart_items>());

        int employeeId = int.Parse(employeeIdStr);

        var cart = _context.EmployeeCarts.FirstOrDefault(c => c.employee_id == employeeId);
        if (cart == null)
            return PartialView("_CartSidebar", new List<cart_items>());

        // Get only this employee's cart items
        var updatedCartItems = _context.CartItems
            .Where(ci => ci.employeecart_id == cart.employeecart_id)
            .Include(ci => ci.Laptop)
            .ThenInclude(l => l!.LaptopDetails)
            .ToList();

        return PartialView("_CartSidebar", updatedCartItems);
    }





}
