using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<WordMap> WordMap { get; set; }
        public DbSet<PageWord> Word { get; set; }
    }
}