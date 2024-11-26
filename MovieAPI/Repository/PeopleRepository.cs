using Microsoft.EntityFrameworkCore;
using MovieAPI.Data.Service;
using MovieAPI.Models;

namespace MovieAPI.Data.Repository;

public class PeopleRepository : IPeopleRepository
{
    private readonly MovieDbContext _context;
    private readonly IMovieDetailsService _movieDetailsService;

    public PeopleRepository(MovieDbContext context, IMovieDetailsService movieDetailsService)
    {
        _context = context;
        _movieDetailsService = movieDetailsService;
    }

    public async Task<IEnumerable<Person>> GetPeopleByName(string name, int maxResults)
    {
        return await _context.People
            .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
            .Take(maxResults)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetMoviesStarredInByPerson(int personId)
    {
        return await _context.Stars
            .Where(s => s.PersonId == personId)
            .Include(s => s.Movie)
            .Select(s => s.Movie)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetMoviesDirectedByPerson(int personId)
    {
        return await _context.Directors
            .Where(d => d.PersonId == personId)
            .Include(d => d.Movie)
            .Select(d => d.Movie)
            .ToListAsync();
    }
    public async Task<int> GetStarredMoviesCountByPerson(int personId)
    {
        return await _context.Stars
            .Where(s => s.PersonId == personId)
            .CountAsync();
    }

    public async Task<int> GetDirectedMoviesCountByPerson(int personId)
    {
        return await _context.Directors
            .Where(d => d.PersonId == personId)
            .CountAsync();
    }
    public async Task<double?> GetAverageRatingForStarredMovies(int personId)
    {
        var query = _context.Ratings
            .Join(
                _context.Stars.Where(s => s.PersonId == personId),
                rating => rating.MovieId,
                star => star.MovieId,
                (rating, star) => rating.Rating
            );

        // Check if there are any entries
        if (!await query.AnyAsync())
        {
            return null; 
        }

        return await query.AverageAsync();
    }

    public async Task<double?> GetAverageRatingForDirectedMovies(int personId)
    {
        var query = _context.Ratings
            .Join(
                _context.Directors.Where(d => d.PersonId == personId),
                rating => rating.MovieId,
                director => director.MovieId,
                (rating, director) => rating.Rating
            );

        // Check if there are any entries 
        if (!await query.AnyAsync())
        {
            return null; 
        }

        return await query.AverageAsync();
    }
    
}