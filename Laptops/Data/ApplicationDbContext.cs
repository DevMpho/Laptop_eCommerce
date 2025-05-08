using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Laptops.Models;

namespace Laptops.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<employee_cart> EmployeeCarts { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<order_status> OrderStatuses { get; set; }
        public DbSet<laptop_details> LaptopDetails { get; set; }
        public DbSet<laptops> Laptops { get; set; }
        public DbSet<cart_item_status> CartItemStatuses { get; set; }
        public DbSet<cart_items> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<employee_cart>().ToTable("employee_cart");
            modelBuilder.Entity<Orders>().ToTable("Orders");
            modelBuilder.Entity<order_status>().ToTable("order_status");
            modelBuilder.Entity<laptop_details>().ToTable("laptop_details");
            modelBuilder.Entity<laptops>().ToTable("Laptops");
            modelBuilder.Entity<cart_item_status>().ToTable("cart_item_status");
            modelBuilder.Entity<cart_items>().ToTable("cart_items");
        }
    }
}