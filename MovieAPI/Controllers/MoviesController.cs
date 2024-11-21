using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieDbContext _context;
        private readonly MovieApiService _movieApiService;
        
        // Declare the API key 
        private readonly string _omdbApiKey = "2b567777";
        
        public MoviesController(MovieDbContext context, MovieApiService movieApiService)
        {
            _context = context;
            _movieApiService = movieApiService;
        }
        // GET: api/movies/name/Last?smartSearch=true&wordComplete=true&limit=10
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchMoviesByName(
            string name,
            [FromQuery] bool smartSearch = false,
            [FromQuery] bool wordComplete = true,
            [FromQuery] int limit = 10)
        {
            var query = _context.Movies.AsQueryable();

            if (smartSearch)
            {
                if (!wordComplete)
                {
                    query = query.Where(m => EF.Functions.Like(m.Title, $"% {name} %") ||
                                             EF.Functions.Like(m.Title, $"{name} %") ||
                                             EF.Functions.Like(m.Title, $"% {name}") ||
                                             m.Title == name);
                }
                else
                {
                    query = query.Where(m => EF.Functions.Like(m.Title, $"%{name}%"));
                }
            }
            else
            {
                query = query.Where(m => m.Title == name);
            }

            var movies = await query.Take(limit).ToListAsync();

            if (!movies.Any())
            {
                return NotFound();
            }

            // Fetch OMDb details for each movie TODO: Make optional (very slow for many movies)
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

            return Ok(movies);
        }

        // GET: api/movies/year/2001?limit=10
        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByYear(int year, [FromQuery] int limit = 10)
        {
            var movies = await _context.Movies
                .Where(m => m.Year == year)
                .Take(limit)
                .ToListAsync();

            if (!movies.Any())
            {
                return NotFound();
            }

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

            return Ok(movies);
        }

        // GET: api/movies/range?start=2001&end=2002&limit=10
        [HttpGet("range")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByYearRange([FromQuery] int? start, [FromQuery] int? end, [FromQuery] int limit = 10)
        {
            if (start == null && end == null)
            {
                return BadRequest("At least one of 'start' or 'end' must be provided.");
            }

            var query = _context.Movies.AsQueryable();

            if (start != null)
            {
                query = query.Where(m => m.Year >= start);
            }

            if (end != null)
            {
                query = query.Where(m => m.Year <= end);
            }

            var movies = await query.Take(limit).ToListAsync();

            if (!movies.Any())
            {
                return NotFound();
            }

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

            return Ok(movies);
        }
        
        // GET: api/movies/omdb/146592
        [HttpGet("omdb/{id}")]
        public async Task<ActionResult<OmdbMovie>> GetMovieFromOmdb(int id)
        {
            try
            {
                var movie = await _movieApiService.GetMovieFromOmdb(id, _omdbApiKey);
                return Ok(movie);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}