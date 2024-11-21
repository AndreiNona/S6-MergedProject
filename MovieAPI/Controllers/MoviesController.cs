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
        
        
        // GET: api/movies/name/Last?smartSearch=true&wordComplete=true&limit=10&includeOmdbDetails=true
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchMoviesByName(
            string name,
            [FromQuery] bool smartSearch = false,
            [FromQuery] bool wordComplete = true,
            [FromQuery] int limit = 10,
            [FromQuery] bool includeOmdbDetails = false)
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

            if (includeOmdbDetails)
            {
                await AddOmdbDetailsToMovies(movies);
            }

            return Ok(movies);
        }


        // GET: api/movies/year/2001?limit=10&includeOmdbDetails=true
        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByYear(int year, [FromQuery] int limit = 10, [FromQuery] bool includeOmdbDetails = false)
        {
            var movies = await _context.Movies
                .Where(m => m.Year == year)
                .Take(limit)
                .ToListAsync();

            if (!movies.Any())
            {
                return NotFound();
            }

            if (includeOmdbDetails)
            {
                await AddOmdbDetailsToMovies(movies);
            }


            return Ok(movies);
        }


        // GET: api/movies/range?start=2001&end=2002&limit=10&includeOmdbDetails=true
        [HttpGet("range")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByYearRange([FromQuery] int? start, [FromQuery] int? end, [FromQuery] int limit = 10, [FromQuery] bool includeOmdbDetails = false)
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

            if (includeOmdbDetails)
            {
                await AddOmdbDetailsToMovies(movies);
            }

            return Ok(movies);
        }
        
        // GET: api/movies/{movieId}/stars
        [HttpGet("{movieId}/stars")]
        public async Task<ActionResult<IEnumerable<Person>>> GetStarsByMovieId(int movieId)
        {
            // Query the Star table 
            var stars = await _context.Stars
                .Where(s => s.MovieId == movieId)
                .Select(s => s.Person)
                .ToListAsync();

            if (stars == null || !stars.Any())
            {
                return NotFound("No stars found for the given movie.");
            }

            return Ok(stars);
        }
        
        // GET: api/movies/{movieId}/directors
        [HttpGet("{movieId}/directors")]
        public async Task<ActionResult<IEnumerable<Person>>> GetDirectorsByMovieId(int movieId)
        {
            // Query the Director table 
            var directors = await _context.Directors
                .Where(d => d.MovieId == movieId)
                .Select(d => d.Person)
                .ToListAsync();

            if (directors == null || !directors.Any())
            {
                return NotFound("No directors found for the given movie.");
            }

            return Ok(directors);
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
}