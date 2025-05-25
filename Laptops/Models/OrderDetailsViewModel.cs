namespace Laptops.Models
{
    public class OrderDetailsViewModel
    {
        // Order info
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
        public int OrderStatusId { get; set; }
        public string Status { get; set; } = string.Empty; // From OrderStatus table

        public int NewStatusId { get; set; }


        // Employee info
        public string EmployeeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long? ContactNumber { get; set; }


        // Laptops in the order
        public List<LaptopViewModel> Laptops { get; set; } = new();
    }
}
