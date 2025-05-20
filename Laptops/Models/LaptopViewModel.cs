namespace Laptops.Models
{
    public class LaptopViewModel
    {
        public int Price { get; set; }
        public string? ImgUrl { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Storage { get; set; }
        public string? Ram { get; set; }
        public string? ScreenSize { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int BatteryLife { get; set; }
        public int LaptopId { get; set; }
        public bool IsInCart { get; set; }

        public string? Role { get; set; }

        public int userLaptopStatus { get; set; } // 0 = available, 1 = in bascket, 
    }
}
