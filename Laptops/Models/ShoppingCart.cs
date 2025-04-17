namespace Laptops.Models
{
    public class ShoppingCart
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public decimal TotalPrice => Products.Sum(p => p.Price) ?? 0;

    }
}
