using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Models;
[Table("ratings")]
public class MovieRating
{
        [Key]
        [Column("movie_id")]
        public int MovieId { get; set; }

        [Column("rating")]
        public double Rating { get; set; }

        [Column("votes")]
        public int Votes { get; set; }
    
}