namespace Laptops.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? LaptopName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedPickupDate { get; set; }
        public string? Status { get; set; }
    }
}
