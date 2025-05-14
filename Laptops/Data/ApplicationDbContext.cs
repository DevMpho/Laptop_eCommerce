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

            // Table name mappings
            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<employee_cart>().ToTable("employee_cart");
            modelBuilder.Entity<Orders>().ToTable("Orders");
            modelBuilder.Entity<order_status>().ToTable("order_status");
            modelBuilder.Entity<laptop_details>().ToTable("laptop_details");
            modelBuilder.Entity<laptops>().ToTable("Laptops");
            modelBuilder.Entity<cart_item_status>().ToTable("cart_item_status");
            modelBuilder.Entity<cart_items>().ToTable("cart_items");

            // ✅ Relationship configurations to prevent EF from guessing wrong column names
            modelBuilder.Entity<cart_items>()
                .HasOne(ci => ci.Order)
                .WithMany()
                .HasForeignKey(ci => ci.order_id);

            modelBuilder.Entity<cart_items>()
                .HasOne(ci => ci.EmployeeCart)
                .WithMany()
                .HasForeignKey(ci => ci.employeecart_id);

            modelBuilder.Entity<cart_items>()
                .HasOne(ci => ci.CartItemStatus)
                .WithMany()
                .HasForeignKey(ci => ci.status_id);

            modelBuilder.Entity<cart_items>()
                .HasOne(ci => ci.Laptop)
                .WithMany()
                .HasForeignKey(ci => ci.laptops_id);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Employee)
                .WithMany()
                .HasForeignKey(o => o.employee_id);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey(o => o.status);
        }

    }
}