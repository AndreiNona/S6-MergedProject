using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Data.Service;

public class MovieDetailsService : IMovieDetailsService
{
    private readonly MovieApiService _movieApiService;
    private readonly string _omdbApiKey;

    public MovieDetailsService(MovieApiService movieApiService)
    {
        _movieApiService = movieApiService;
        _omdbApiKey = "2b567777"; // Ideally, store this in configuration
    }

    public async Task AddOmdbDetailsToMovies(IEnumerable<Movie> movies)
    {
        var tasks = movies.Select(async movie =>
        {
            try
            {
                var (poster, genre, ratings) = await _movieApiService.GetMovieDetailsFromOmdb(movie.Id, _omdbApiKey);
                movie.Poster = poster;
                movie.Genre = genre;
                movie.Ratings = ratings;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to fetch OMDb data for movie ID {movie.Id}: {ex.Message}");
            }
        });

        await Task.WhenAll(tasks);
    }


    public async Task<OmdbMovie> GetMovieFromOmdb(int id)
    {
        try
        {
            var omdbMovie = await _movieApiService.GetMovieFromOmdb(id, _omdbApiKey);
            return omdbMovie;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Failed to fetch OMDb data for movie ID {id}: {ex.Message}");
            throw;
        }
    }
}