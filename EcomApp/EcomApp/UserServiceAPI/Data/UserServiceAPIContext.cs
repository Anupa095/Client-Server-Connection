using Microsoft.EntityFrameworkCore;
using UserServiceAPI.Models;

namespace UserServiceAPI.Data
{
    public class UserServiceAPIContext : DbContext
    {
        public UserServiceAPIContext(DbContextOptions<UserServiceAPIContext> options)
            : base(options)
        {
        }

        // ✅ The DbSet must match what you use in your code
        public DbSet<User> User { get; set; } = default!;
    }
}
