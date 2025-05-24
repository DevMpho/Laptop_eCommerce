namespace Laptops.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
        public string Status { get; set; } // from order_status

        public string Email { get; set; }
        public List<LaptopViewModel> Laptops { get; set; } = new();
    }
}