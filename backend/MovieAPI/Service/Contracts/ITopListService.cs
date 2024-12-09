using MovieAPI.Models;

namespace MovieAPI.Data.Service;

public interface ITopListService
{
    Task AddTopListAsync(string userId, string topListName, List<int> movieIds);
    Task<IEnumerable<TopList>> GetUserTopListsAsync(string userId);
    Task<TopList> GetTopListByIdAsync(int topListId, string userId);
    Task UpdateTopListAsync(int topListId, string userId, List<int> movieIds);
    Task DeleteTopListAsync(int topListId, string userId);
    
}