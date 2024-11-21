using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Star> Stars { get; set; }
        public DbSet<Director> Directors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mark as a keyless entity
            modelBuilder.Entity<Rating>().HasNoKey();
            modelBuilder.Entity<Star>().HasNoKey();
            modelBuilder.Entity<Director>().HasNoKey();
        }
    }
}