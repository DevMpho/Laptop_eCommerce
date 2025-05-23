using Laptops.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


public class Orders
{
    [Key]
    public int order_id { get; set; }

    public int? employee_id { get; set; }  // Make nullable
    public DateTime order_date { get; set; }
    public int total_amount { get; set; }

    public int? order_status_id { get; set; }  // Make nullable

    [ForeignKey(nameof(order_status_id))]
    public order_status? OrderStatus { get; set; }  // Make nullable

    [ForeignKey(nameof(employee_id))]
    public Employee? Employee { get; set; } // Make nullable

    public ICollection<cart_items>? CartItems { get; set; }
}
