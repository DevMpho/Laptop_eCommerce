namespace Laptops.Models
{
    public class Orders
    {
        public int order_id { get; set; }
        public int employee_id { get; set; }
        public DateTime order_date { get; set; }
        public decimal totalAmount { get; set; }
        public int status { get; set; }
        public order_status OrderStatus { get; set; }
        public Employee Employee { get; set; }
    }
}
