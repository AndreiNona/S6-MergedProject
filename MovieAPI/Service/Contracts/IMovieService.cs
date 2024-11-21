using MovieAPI.Models;

namespace MovieAPI.Data.Service;

public interface IMovieService
{
    Task<IEnumerable<Movie>> SearchMoviesByName(string name, bool smartSearch, bool wordComplete, int limit, bool includeOmdbDetails);
    Task<IEnumerable<Movie>> GetMoviesByYear(int year, int limit, bool includeOmdbDetails);
    Task<IEnumerable<Movie>> GetMoviesByYearRange(int? start, int? end, int limit, bool includeOmdbDetails);
    Task<IEnumerable<Person>> GetStarsByMovieId(int movieId);
    Task<IEnumerable<Person>> GetDirectorsByMovieId(int movieId);
    Task<Movie> GetMovieById(int movieId);  
    Task<OmdbMovie> GetMovieFromOmdb(int id);

}