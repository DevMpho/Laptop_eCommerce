using System.ComponentModel.DataAnnotations;

namespace Laptops.Models
{
    public class cart_item_status
    {
        [Key]
        public int status_id { get; set; }  
        public string? status_name { get; set; }
    }
}
