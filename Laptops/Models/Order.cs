using Laptops.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Orders
{
    [Key]
    public int order_id { get; set; }

    public int employee_id { get; set; }
    public DateTime order_date { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal totalAmount { get; set; }
    public int status { get; set; }

    [ForeignKey(nameof(status))]
    public required order_status OrderStatus { get; set; }

    [ForeignKey(nameof(employee_id))]
    public required Employee Employee { get; set; }
}
