using MovieAPI.Models;

namespace MovieAPI.Data.Service;

public interface IPeopleService
{
    Task<IEnumerable<Person>> GetPeopleByName(string name, int maxResults);
    Task<IEnumerable<Movie>> GetMoviesStarredInByPerson(int personId, bool includeOmdbDetails = false);
    Task<IEnumerable<Movie>> GetMoviesDirectedByPerson(int personId, bool includeOmdbDetails = false);
    Task<object> GetPersonRole(int personId);
}