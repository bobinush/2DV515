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
        // Project
        public DbSet<GenreP> GenresP { get; set; }
        public DbSet<TagP> TagsP { get; set; }
        public DbSet<MovieGenreP> MovieGenresP { get; set; }
        public DbSet<MovieP> MoviesP { get; set; }
        public DbSet<UserP> UsersP { get; set; }
        public DbSet<RatingP> RatingsP { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieGenreP>()
                .HasKey(t => new { t.MovieId, t.GenreId });
            modelBuilder.Entity<RatingP>()
                .HasKey(t => new { t.UserId, t.MovieId });
            modelBuilder.Entity<MovieP>().Property(t => t.Id)
                .ValueGeneratedNever();
        }
    }

}