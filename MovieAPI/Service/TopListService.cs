using System.ComponentModel.DataAnnotations;
using MovieAPI.Data.Repository;
using MovieAPI.Models;

namespace MovieAPI.Data.Service;

public class TopListService : ITopListService
{
    private readonly ITopListRepository _topListRepository;

    public TopListService(ITopListRepository topListRepository)
    {
        _topListRepository = topListRepository;
    }

    public async Task AddTopListAsync(string userId, string topListName, List<int> movieIds)
    {
        var topList = new TopList
        {
            Name = topListName,
            UserId = userId,
            MovieIds = movieIds
        };
        
        await _topListRepository.AddTopListAsync(topList);
    }

    public async Task<IEnumerable<TopList>> GetUserTopListsAsync(string userId)
    {
        return await _topListRepository.GetTopListsByUserIdAsync(userId);
    }

    public async Task<TopList> GetTopListByIdAsync(int topListId, string userId)
    {
        var topList = await _topListRepository.GetTopListByIdAsync(topListId);
        if (topList?.UserId == userId)
        {
            return topList;
        }
        return null;
    }

    public async Task UpdateTopListAsync(int topListId, string userId, List<int> movieIds)
    {
        // Fetch the top list
        var topList = await GetTopListByIdAsync(topListId, userId);
        if (topList == null)
        {
            throw new KeyNotFoundException($"Top list with ID {topListId} not found for user {userId}.");
        }

        // Validate 
        if (movieIds.Distinct().Count() != movieIds.Count)
        {
            throw new ValidationException("The top list contains duplicate movies. Please remove duplicates before updating.");
        }

        // Update the top list
        topList.MovieIds = movieIds;
        await _topListRepository.UpdateTopListAsync(topList);
    }

    public async Task DeleteTopListAsync(int topListId, string userId)
    {
        var topList = await GetTopListByIdAsync(topListId, userId);
        if (topList != null)
        {
            await _topListRepository.DeleteTopListAsync(topListId);
        }
    }
}