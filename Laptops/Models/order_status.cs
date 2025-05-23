using System.ComponentModel.DataAnnotations;

namespace Laptops.Models
{
    public class order_status
    {
        [Key]
        public int order_status_id { get; set; }
        public string status_name { get; set; }
    }
}
