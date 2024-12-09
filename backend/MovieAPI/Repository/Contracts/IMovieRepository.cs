using MovieAPI.Models;

namespace MovieAPI.Data.Repository;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> SearchMoviesByName(string name, bool smartSearch, bool wordComplete, int limit);
    Task<IEnumerable<Movie>> GetMoviesByYear(int year, int limit);
    Task<IEnumerable<Movie>> GetMoviesByYearRange(int? start, int? end, int limit);
    Task<IEnumerable<Person>> GetStarsByMovieId(int movieId);
    Task<IEnumerable<Person>> GetDirectorsByMovieId(int movieId);
    Task<Movie> GetMovieById(int movieId);  
    Task<IEnumerable<Movie>> GetMoviesByName(string name);
}