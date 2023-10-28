using GRPC.Todo.Models;
using Microsoft.EntityFrameworkCore;

namespace GRPC.Todo.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
            
        }
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    }
}
