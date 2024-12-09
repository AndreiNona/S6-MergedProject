namespace MovieAPI.Models;

public class MovieListing
{
    public int Id { get; set; } // Primary key
    public int Rank { get; set; } 
    public int MovieId { get; set; } // Reference 
    public string UserId { get; set; } // Reference
    
    public ApplicationUser User { get; set; } // Navigation property
}