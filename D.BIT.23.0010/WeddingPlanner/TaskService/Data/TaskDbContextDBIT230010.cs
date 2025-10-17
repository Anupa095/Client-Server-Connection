using Microsoft.EntityFrameworkCore;
using TaskService.Models;

namespace TaskService.Data
{
    public class TaskDbContextDBIT230010 : DbContext
    {
        public TaskDbContextDBIT230010(DbContextOptions<TaskDbContextDBIT230010> options)
            : base(options) { }

        public DbSet<TaskDBIT230010> Tasks => Set<TaskDBIT230010>();
    }
}
