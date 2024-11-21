using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Models;

public class RatingEntry
{
    public int MovieId { get; set; }
    public float Rating { get; set; } // Actual rating value (e.g., 8.5)
    public int Votes { get; set; } // Number of votes

    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }
}