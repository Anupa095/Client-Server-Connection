using Microsoft.EntityFrameworkCore;
using OrderServiceAPI.Models;

namespace OrderServiceAPI.Data
{
    public class OrderServiceAPIContext : DbContext
    {
        public OrderServiceAPIContext(DbContextOptions<OrderServiceAPIContext> options)
            : base(options)
        {
        }

        // Use plural 'Orders' to follow EF Core convention
        public DbSet<Order> Orders { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: explicitly map table if needed
            modelBuilder.Entity<Order>().ToTable("Orders");
        }
    }
}
