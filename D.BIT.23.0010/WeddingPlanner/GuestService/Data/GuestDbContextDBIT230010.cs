using Microsoft.EntityFrameworkCore;
using GuestService.Model;

namespace GuestService.Data
{
    public class GuestDbContextDBIT230010 : DbContext
    {
        public GuestDbContextDBIT230010(DbContextOptions<GuestDbContextDBIT230010> options)
            : base(options) { }

        public DbSet<GuestDBIT230010> Guests { get; set; }
    }
}
