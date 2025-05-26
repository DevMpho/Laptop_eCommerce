using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Laptops.Models;
using Laptops.Data;
using Laptops.Helpers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly LaptopService _laptopService;
    private readonly LaptopStatusHelper _laptopStatusHelper;

    public CartController(ApplicationDbContext context, LaptopService laptopService, LaptopStatusHelper laptopStatusHelper)
    {
        _context = context;
        _laptopService = laptopService;
        _laptopStatusHelper = laptopStatusHelper;
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
        bool updated = _laptopService.AddToCart(laptopId); // 1 = in basket

        if (updated)
            return Json(new { success = true });
        else
            return Json(new { success = false, message = "Could not update laptop status." });
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
                            .Where(ci => ci.employeecart_id == cart.employeecart_id && ci.order_id == null)
                            .Count();
        }

        return Json(new { count });
    }


    public IActionResult Sidebar()
    {
        var employeeIdStr = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrEmpty(employeeIdStr))
            return PartialView("_CartSidebar", new List<cart_items>());

        int employeeId = int.Parse(employeeIdStr);
        var cart = _context.EmployeeCarts.FirstOrDefault(c => c.employee_id == employeeId);
        if (cart == null)
            return PartialView("_CartSidebar", new List<cart_items>());

        var items = _context.CartItems
            .Where(ci => ci.employeecart_id == cart.employeecart_id && ci.order_id == null)  // Exclude ordered items
            .Include(ci => ci.Laptop)
            .ThenInclude(l => l!.LaptopDetails)
            .ToList();

        return PartialView("_CartSidebar", items);
    }


    [HttpPost]
    public IActionResult RemoveFromCart(int id)
    {
        var item = _context.CartItems
            .Include(ci => ci.Laptop) // Include Laptop to access laptopId
            .FirstOrDefault(ci => ci.cartitems_id == id);

        if (item != null)
        {
            int? laptopId = item.Laptop?.laptops_id;

            _context.CartItems.Remove(item);
            _context.SaveChanges();

            // ✅ Update status in cache to available
            if (laptopId.HasValue)
            {
                _laptopService.DeleteCartItem(laptopId.Value); // 0 = available
            }
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
    [HttpPost]
    public async Task<IActionResult> Orders(int totalAmount)
    {
        var employeeIdStr = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrEmpty(employeeIdStr))
            return Unauthorized("User not logged in.");

        int employeeId = int.Parse(employeeIdStr);

        var employeeCart = _context.EmployeeCarts.FirstOrDefault(c => c.employee_id == employeeId);
        if (employeeCart == null)
            return BadRequest("Employee cart not found.");

        var cartItems = _context.CartItems
            .Where(ci => ci.employeecart_id == employeeCart.employeecart_id && ci.order_id == null)
            .ToList();

        if (!cartItems.Any())
            return BadRequest("No items in cart to place order.");

        var newOrder = new Orders
        {
            employee_id = employeeId,
            order_date = DateTime.Now,
            order_status_id = 1, // Pending
            total_amount = totalAmount,
        };

        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();

        // Assign order_id to cart items
        foreach (var item in cartItems)
        {
            item.order_id = newOrder.order_id;
            item.status_id = 2;  // now “ordered”
            _laptopService.AddToOrders(item.laptops_id); // updates laptop cache status
        }
        await _context.SaveChangesAsync();

        // ─── invalidate the cached orders for this user ─────
        _laptopStatusHelper.InvalidateEmployeeOrdersCache(employeeId);

        return RedirectToAction("Orders", "Home");
    }

    //MSP orders




}


