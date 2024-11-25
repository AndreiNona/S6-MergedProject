using MovieAPI.Models;

namespace MovieAPI.Data.Repository;

public interface IPeopleRepository
{
    Task<IEnumerable<Person>> GetPeopleByName(string name, int maxResults);
    Task<IEnumerable<Movie>> GetMoviesStarredInByPerson(int personId);
    Task<IEnumerable<Movie>> GetMoviesDirectedByPerson(int personId);
    Task<int> GetStarredMoviesCountByPerson(int personId);
    Task<int> GetDirectedMoviesCountByPerson(int personId);
}