using Microsoft.EntityFrameworkCore;
using ProductServiceAPI.Models;

namespace ProductServiceAPI.Data
{
    public class ProductServiceAPIContext : DbContext
    {
        public ProductServiceAPIContext(DbContextOptions<ProductServiceAPIContext> options)
            : base(options)
        {
        }

        // Plural DbSet for convention
        public DbSet<Product> Products { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit mapping (optional)
            modelBuilder.Entity<Product>().ToTable("Products");
        }
    }
}
