using Microsoft.EntityFrameworkCore;
using EventService.Models;

namespace EventService.Data
{
    public class EventDbContextDBIT230010 : DbContext
    {
        public EventDbContextDBIT230010(DbContextOptions<EventDbContextDBIT230010> options)
            : base(options)
        {
        }

        public DbSet<EventDBIT230010> Events => Set<EventDBIT230010>();
    }
}
