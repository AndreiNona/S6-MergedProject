using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Models;

public class Director
{
    public int MovieId { get; set; }
    public int PersonId { get; set; }

    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    [ForeignKey("PersonId")]
    public Person Person { get; set; }
}