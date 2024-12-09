using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data.Repository;

public class TopListRepository : ITopListRepository
{
    private readonly MovieDbContext _context;

    public TopListRepository(MovieDbContext context)
    {
        _context = context;
    }

    public async Task AddTopListAsync(TopList topList)
    {
        _context.TopLists.Add(topList);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TopList>> GetTopListsByUserIdAsync(string userId)
    {
        return await _context.TopLists
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<TopList> GetTopListByIdAsync(int topListId)
    {
        return await _context.TopLists.FindAsync(topListId);
    }

    public async Task UpdateTopListAsync(TopList topList)
    {
        _context.TopLists.Update(topList);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTopListAsync(int topListId)
    {
        var topList = await GetTopListByIdAsync(topListId);
        if (topList != null)
        {
            _context.TopLists.Remove(topList);
            await _context.SaveChangesAsync();
        }
    }
}