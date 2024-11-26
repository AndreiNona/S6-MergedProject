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
    public async Task<object> GetPersonRole(int personId)
    {
        var starredMoviesCount = await _peopleRepository.GetStarredMoviesCountByPerson(personId);
        var directedMoviesCount = await _peopleRepository.GetDirectedMoviesCountByPerson(personId);

        var role = directedMoviesCount > starredMoviesCount ? "Director" : "Star";

        var starredMovies = await _peopleRepository.GetMoviesStarredInByPerson(personId);
        var directedMovies = await _peopleRepository.GetMoviesDirectedByPerson(personId);

        if (!starredMovies.Any() && !directedMovies.Any())
        {
            return null;
        }

        return new
        {
            Role = role,
            DirectedMovies = directedMovies,
            StarredMovies = starredMovies
        };
    }
    public async Task<object> GetAverageRatingsForPerson(int personId)
    {
        var averageRatingStarred = await _peopleRepository.GetAverageRatingForStarredMovies(personId);
        var averageRatingDirected = await _peopleRepository.GetAverageRatingForDirectedMovies(personId);

        // Format the ratings and handle missing entries
        var formattedStarredRating = averageRatingStarred.HasValue
            ? Math.Round(averageRatingStarred.Value, 2).ToString("F2") // Format to 2 decimal places
            : "N/A";

        var formattedDirectedRating = averageRatingDirected.HasValue
            ? Math.Round(averageRatingDirected.Value, 2).ToString("F2") // Format to 2 decimal places
            : "N/A";

        return new
        {
            AverageRatingStarred = formattedStarredRating,
            AverageRatingDirected = formattedDirectedRating
        };
    }
}