using Microsoft.AspNetCore.Identity;

namespace MovieAPI.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<MovieListing> MovieListings { get; set; } = new List<MovieListing>();
}