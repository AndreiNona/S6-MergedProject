using MovieAPI.Models;

namespace MovieAPI.Data.Repository;

public interface ITopListRepository
{
    Task AddTopListAsync(TopList topList);
    Task<IEnumerable<TopList>> GetTopListsByUserIdAsync(string userId);
    Task<TopList> GetTopListByIdAsync(int topListId);
    Task UpdateTopListAsync(TopList topList);
    Task DeleteTopListAsync(int topListId);
}