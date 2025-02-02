using Microsoft.EntityFrameworkCore;
using PeopleApi.Data;
using PeopleApi.Models;

namespace PeopleApi.Repositories;

public interface IFriendshipRepository
{
    Task<List<User>> GetFriendsByUserIdAsync(int userId);
    Task<bool> AddFriendAsync(int userId, int friendId);
    Task<bool> RemoveFriendAsync(int userId, int friendId);
}
public class FriendshipRepository : IFriendshipRepository
{
    private readonly PeopleApiDbContext _context;

    public FriendshipRepository(PeopleApiDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetFriendsByUserIdAsync(int userId)
    {
        return await _context.Friendships
            .Where(f => f.UserId == userId)
            .Select(f => f.Friend)
            .ToListAsync();
    }

    public async Task<bool> AddFriendAsync(int userId, int friendId)
    {
        if (await _context.Friendships.AnyAsync(f => f.UserId == userId && f.FriendId == friendId))
            return false;

        _context.Friendships.Add(new Friendship { UserId = userId, FriendId = friendId });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFriendAsync(int userId, int friendId)
    {
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);

        if (friendship == null)
            return false;

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();
        return true;
    }
}