using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data.Repository;

public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _context;

        public MovieRepository(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> SearchMoviesByName(string name, bool smartSearch, bool wordComplete, int limit)
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

            return await query.Take(limit).ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByYear(int year, int limit)
        {
            return await _context.Movies
                .Where(m => m.Year == year)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByYearRange(int? start, int? end, int limit)
        {
            var query = _context.Movies.AsQueryable();

            if (start != null)
            {
                query = query.Where(m => m.Year >= start);
            }

            if (end != null)
            {
                query = query.Where(m => m.Year <= end);
            }

            return await query.Take(limit).ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetStarsByMovieId(int movieId)
        {
            return await _context.Stars
                .Where(s => s.MovieId == movieId)
                .Select(s => s.Person)
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetDirectorsByMovieId(int movieId)
        {
            return await _context.Directors
                .Where(d => d.MovieId == movieId)
                .Select(d => d.Person)
                .ToListAsync();
        }

        public async Task<Movie> GetMovieById(int movieId) 
        {
            return await _context.Movies.FindAsync(movieId);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByName(string name) 
        {
            return await _context.Movies
                .Where(m => EF.Functions.Like(m.Title, $"%{name}%"))
                .ToListAsync();
        }
}