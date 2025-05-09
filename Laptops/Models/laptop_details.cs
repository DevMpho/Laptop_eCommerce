using System.ComponentModel.DataAnnotations;

namespace Laptops.Models
{
    public class laptop_details
    {
        [Key]
        public int laptopdetails_id { get; set; }
        public string? brand { get; set; }
        public string? model { get; set; }
        public string? processor { get; set; }
        public int storage { get; set; }
        public int ram { get; set; }
        public string? role { get; set; }
    }
}
