namespace Laptops.Models
{
    public class ShoppingCart
    {
        public List<Product2> Products { get; set; } = new List<Product2>();
        public decimal TotalPrice => Products.Sum(p => p.Price) ?? 0;

    }
}
