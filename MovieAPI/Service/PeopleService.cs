using MovieAPI.Data.Repository;
using MovieAPI.Models;

namespace MovieAPI.Data.Service;

public class PeopleService : IPeopleService
{
    private readonly IPeopleRepository _peopleRepository;
    private readonly IMovieDetailsService _movieDetailsService;

    public PeopleService(IPeopleRepository peopleRepository, IMovieDetailsService movieDetailsService)
    {
        _peopleRepository = peopleRepository ?? throw new ArgumentNullException(nameof(peopleRepository));
        _movieDetailsService = movieDetailsService ?? throw new ArgumentNullException(nameof(movieDetailsService));
    }

    public async Task<IEnumerable<Person>> GetPeopleByName(string name, int maxResults)
    {
        return await _peopleRepository.GetPeopleByName(name, maxResults);
    }

    public async Task<IEnumerable<Movie>> GetMoviesStarredInByPerson(int personId, bool includeOmdbDetails = false)
    {
        var movies = await _peopleRepository.GetMoviesStarredInByPerson(personId);

        if (includeOmdbDetails)
        {
            if (_movieDetailsService == null)
            {
                throw new InvalidOperationException($"{nameof(_movieDetailsService)} is not properly initialized.");
            }

            await _movieDetailsService.AddOmdbDetailsToMovies(movies);
        }

        return movies;
    }

    public async Task<IEnumerable<Movie>> GetMoviesDirectedByPerson(int personId, bool includeOmdbDetails = false)
    {
        var movies = await _peopleRepository.GetMoviesDirectedByPerson(personId);

        if (includeOmdbDetails)
        {
            if (_movieDetailsService == null)
            {
                throw new InvalidOperationException($"{nameof(_movieDetailsService)} is not properly initialized.");
            }

            await _movieDetailsService.AddOmdbDetailsToMovies(movies);
        }

        return movies;
    }
}