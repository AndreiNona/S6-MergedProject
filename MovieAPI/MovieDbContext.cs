using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data;

public class MovieDbContext : IdentityDbContext<ApplicationUser>
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Star> Stars { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<TopList> TopLists { get; set; } 
    public DbSet<MovieRating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define additional relationships or configurations if needed
        modelBuilder.Entity<Rating>().HasNoKey();
        modelBuilder.Entity<Star>().HasNoKey();
        modelBuilder.Entity<Director>().HasNoKey();
    }
}