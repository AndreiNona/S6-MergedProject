using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Models;

public class Director
{
    [Key]
    [Column("movie_id", Order = 0)]
    public int MovieId { get; set; }
    [Key]
    [Column("person_id", Order = 1)]
    public int PersonId { get; set; }

    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    [ForeignKey("PersonId")]
    public Person Person { get; set; }
}