using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Data.Service;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
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
            var movies = await _movieService.SearchMoviesByName(name, smartSearch, wordComplete, limit, includeOmdbDetails);

            if (!movies.Any())
            {
                return NotFound();
            }

            return Ok(movies);
        }


        // GET: api/movies/year/2001?limit=10&includeOmdbDetails=true
        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByYear(int year, [FromQuery] int limit = 10, [FromQuery] bool includeOmdbDetails = false)
        {
            var movies = await _movieService.GetMoviesByYear(year, limit, includeOmdbDetails);

            if (!movies.Any())
            {
                return NotFound();
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

            var movies = await _movieService.GetMoviesByYearRange(start, end, limit, includeOmdbDetails);

            if (!movies.Any())
            {
                return NotFound();
            }

            return Ok(movies);
        }
        
        // GET: api/movies/{movieId}/stars
        [HttpGet("{movieId}/stars")]
        public async Task<ActionResult<IEnumerable<Person>>> GetStarsByMovieId(int movieId)
        {
            var stars = await _movieService.GetStarsByMovieId(movieId);

            if (!stars.Any())
            {
                return NotFound("No stars found for the given movie.");
            }

            return Ok(stars);
        }
        
        // GET: api/movies/{movieId}/directors
        [HttpGet("{movieId}/directors")]
        public async Task<ActionResult<IEnumerable<Person>>> GetDirectorsByMovieId(int movieId)
        {
            var directors = await _movieService.GetDirectorsByMovieId(movieId);

            if (!directors.Any())
            {
                return NotFound("No directors found for the given movie.");
            }

            return Ok(directors);
        }
        
        // GET: api/movies/omdb/146592
        [HttpGet("omdb/{id}")]
        public async Task<ActionResult<OmdbMovie>> GetMovieFromOmdb(int id)
        {
            var movie = await _movieService.GetMovieFromOmdb(id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }
        
        private async Task AddOmdbDetailsToMovies(IEnumerable<Movie> movies)
        {
            var tasks = movies.Select(async movie =>
            {
                try
                {
                    var omdbMovie = await _movieService.GetMovieFromOmdb(movie.Id);
                    if (omdbMovie != null)
                    {
                        movie.Poster = omdbMovie.Poster;
                        movie.Genre = omdbMovie.Genre;
                        movie.Ratings = omdbMovie.Ratings;
                    }
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