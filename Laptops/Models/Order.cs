using Laptops.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Orders
{
    [Key]
    public int order_id { get; set; }

    public int employee_id { get; set; }
    public DateTime order_date { get; set; }

    public int total_amount { get; set; }
    // Foreign key to order_status
    public int order_status_id { get; set; }

    [ForeignKey(nameof(order_status_id))]
    public required order_status OrderStatus { get; set; }

    [ForeignKey(nameof(employee_id))]
    public required Employee Employee { get; set; }

    public ICollection<cart_items> CartItems { get; set; }

}
