using MovieAPI.Models;

namespace MovieAPI.Data.Service;

public interface IMovieDetailsService
{
    Task AddOmdbDetailsToMovies(IEnumerable<Movie> movies);
    Task<OmdbMovie> GetMovieFromOmdb(int id);
}