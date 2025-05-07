namespace Laptops.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }

        public string Brand { get; set; }
        public string Storage { get; set; }

        public string Memory { get; set; }
    }
}
