using MovieAPI.Data.Repository;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Data.Service;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IMovieDetailsService _movieDetailsService;

    public MovieService(IMovieRepository movieRepository, IMovieDetailsService movieDetailsService)
    {
        _movieRepository = movieRepository;
        _movieDetailsService = movieDetailsService;
    }

    public async Task<IEnumerable<Movie>> SearchMoviesByName(string name, bool smartSearch, bool wordComplete, int limit, bool includeOmdbDetails)
    {
        var movies = await _movieRepository.SearchMoviesByName(name, smartSearch, wordComplete, limit);

        if (includeOmdbDetails)
        {
            await _movieDetailsService.AddOmdbDetailsToMovies(movies);
        }

        return movies;
    }

    public async Task<IEnumerable<Movie>> GetMoviesByYear(int year, int limit, bool includeOmdbDetails)
    {
        var movies = await _movieRepository.GetMoviesByYear(year, limit);

        if (includeOmdbDetails)
        {
            await _movieDetailsService.AddOmdbDetailsToMovies(movies);
        }

        return movies;
    }

    public async Task<IEnumerable<Movie>> GetMoviesByYearRange(int? start, int? end, int limit, bool includeOmdbDetails)
    {
        var movies = await _movieRepository.GetMoviesByYearRange(start, end, limit);

        if (includeOmdbDetails)
        {
            await _movieDetailsService.AddOmdbDetailsToMovies(movies);
        }

        return movies;
    }

    public async Task<IEnumerable<Person>> GetStarsByMovieId(int movieId)
    {
        return await _movieRepository.GetStarsByMovieId(movieId);
    }

    public async Task<IEnumerable<Person>> GetDirectorsByMovieId(int movieId)
    {
        return await _movieRepository.GetDirectorsByMovieId(movieId);
    }

    public async Task<Movie> GetMovieById(int movieId)
    {
        return await _movieRepository.GetMovieById(movieId);
    }

    public async Task<OmdbMovie> GetMovieFromOmdb(int id)
    {
        return await _movieDetailsService.GetMovieFromOmdb(id); // Updated to use IMovieDetailsService
    }
}
