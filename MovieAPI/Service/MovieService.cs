using MovieAPI.Data.Repository;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Data.Service;

public class MovieService : IMovieService
{
        private readonly IMovieRepository _movieRepository;
        private readonly MovieApiService _movieApiService;

        private readonly string _omdbApiKey = "2b567777";  // This can be managed via appsettings.

        public MovieService(IMovieRepository movieRepository, MovieApiService movieApiService)
        {
            _movieRepository = movieRepository;
            _movieApiService = movieApiService;
        }

        public async Task<IEnumerable<Movie>> SearchMoviesByName(string name, bool smartSearch, bool wordComplete, int limit, bool includeOmdbDetails)
        {
            var movies = await _movieRepository.SearchMoviesByName(name, smartSearch, wordComplete, limit);

            if (includeOmdbDetails)
            {
                await AddOmdbDetailsToMovies(movies);
            }

            return movies;
        }

        public async Task<IEnumerable<Movie>> GetMoviesByYear(int year, int limit, bool includeOmdbDetails)
        {
            var movies = await _movieRepository.GetMoviesByYear(year, limit);

            if (includeOmdbDetails)
            {
                await AddOmdbDetailsToMovies(movies);
            }

            return movies;
        }

        public async Task<IEnumerable<Movie>> GetMoviesByYearRange(int? start, int? end, int limit, bool includeOmdbDetails)
        {
            var movies = await _movieRepository.GetMoviesByYearRange(start, end, limit);

            if (includeOmdbDetails)
            {
                await AddOmdbDetailsToMovies(movies);
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

        public async Task<Movie> GetMovieById(int movieId) // Add this implementation
        {
            return await _movieRepository.GetMovieById(movieId);
        }

        public async Task<OmdbMovie> GetMovieFromOmdb(int id)
        {
            return await _movieApiService.GetMovieFromOmdb(id, _omdbApiKey);
        }

        private async Task AddOmdbDetailsToMovies(IEnumerable<Movie> movies)
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
        
}