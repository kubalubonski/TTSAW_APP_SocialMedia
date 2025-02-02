using PeopleApi.Models;
using PeopleApi.Data;
using Microsoft.EntityFrameworkCore;

namespace PeopleApi.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<List<User>> GetUsersNotInListAsync(List<int> userIds);
}
public class UserRepository : IUserRepository
{
    private readonly PeopleApiDbContext _context;

    public UserRepository(PeopleApiDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<List<User>> GetUsersNotInListAsync(List<int> userIds)
    {
        return await _context.Users
            .Where(u => !userIds.Contains(u.Id))
            .ToListAsync();
    }
}