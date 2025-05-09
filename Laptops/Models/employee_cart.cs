using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laptops.Models
{
    public class employee_cart
    {
        [Key]
        public int employeecart_id { get; set; }
        [ForeignKey("employee")]
        public int employee_id { get; set; }
        public DateTime created_at { get; set; }
        public required Employee Employee { get; set; }
    }
}
