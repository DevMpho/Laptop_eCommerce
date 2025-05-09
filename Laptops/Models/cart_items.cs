using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laptops.Models
{
    public class cart_items
    {
        [Key]
        public int cartitems_id { get; set; }
        [ForeignKey("laptops")]
        public int laptops_id { get; set; }
        [ForeignKey("cart_item_status")]
        public int status_id { get; set; }
        [ForeignKey("employee_cart")]
        public int employeecart_id { get; set; }
        [ForeignKey("orders")]
        public int order_id { get; set; }
        public required cart_item_status CartItemStatus { get; set; }
        public required laptops Laptop { get; set; }
        public required employee_cart EmployeeCart { get; set; }
        public required Orders Order { get; set; }
    }
}
