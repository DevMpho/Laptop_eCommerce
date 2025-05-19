using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laptops.Models
{
    public class laptops
    {
        [Key]
        public int laptops_id { get; set; }

        // Explicitly specify the column name
        [ForeignKey("LaptopDetails")]
        [Column("laptopdetails_id")]
        public int laptopdetails_id { get; set; }

        public int price { get; set; }
        public int batteryLife { get; set; }
        public string? screensize { get; set; }
        public string? description { get; set; }
        public string? color { get; set; }
        public string? imgUrl { get; set; }
        public required laptop_details LaptopDetails { get; set; }
    }
}
