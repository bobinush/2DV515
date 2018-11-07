using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
    }
}