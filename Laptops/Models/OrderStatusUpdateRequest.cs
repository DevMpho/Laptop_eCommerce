namespace Laptops.Models
{
    public class OrderStatusUpdateRequest
    {
        public int OrderId { get; set; }
        public int NewStatusId { get; set; }

    }
}
