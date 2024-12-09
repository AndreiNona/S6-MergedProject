using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        [NotMapped]
        public string Poster { get; set; } // OMDb 
        [NotMapped]
        public string Genre { get; set; } // OMDb 
        [NotMapped]
        public List<Rating> Ratings { get; set; } // OMDb 
    }
    
}