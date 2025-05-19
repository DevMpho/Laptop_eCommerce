using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laptops.Models
{
    public class cart_items
    {
        [Key]
        public int cartitems_id { get; set; }

        // Foreign keys should reference the navigation property names, not table names
        [ForeignKey("Laptop")]
        public int laptops_id { get; set; }

        [ForeignKey("CartItemStatus")]
        public int status_id { get; set; }

        [ForeignKey("EmployeeCart")]
        public int employeecart_id { get; set; }

        [ForeignKey("Order")]
        public int? order_id { get; set; }  // ✅ Nullable foreign key

        // Navigation properties should be nullable unless you ensure they are always set in code
        public cart_item_status? CartItemStatus { get; set; }
        public laptops? Laptop { get; set; }
        
        public employee_cart? EmployeeCart { get; set; }
        public Orders? Order { get; set; }
        

    }
}
