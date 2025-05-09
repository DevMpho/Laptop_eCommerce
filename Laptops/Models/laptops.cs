using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laptops.Models
{
    public class laptops
    {
        [Key]
        public int laptops_id { get; set; }
        [ForeignKey("laptop_details")]
        public int laptopdetails_id { get; set; }
        public decimal price { get; set; }
        public string? batteryLife { get; set; }
        public string? screensize { get; set; }
        public string? description { get; set; }
        public string? color { get; set; }
        public string? imgUrl { get; set; }
        public required laptop_details LaptopDetails { get; set; }
    }
}
