using Microsoft.EntityFrameworkCore;
using PostApi.Models;
using PostApi.Data;

namespace PostApi.Repositories;


public interface IPostRepository
{
    Task<List<Post>> GetAllPosts();
    Task<Post> CreatePostAsync(Post post);
    Task<Post> GetPostByIdAsync(int id);
    Task<List<Post>> GetMyPostsAsync(int userId);
    Task<List<Post>> GetFriendsPostsAsync(int userId);
    Task DeletePostAsync(Post post);
}

public class PostRepository : IPostRepository
{
    private readonly PostApiDbContext _context;
    private readonly FriendshipDbContext _friends;

    public PostRepository(PostApiDbContext context, FriendshipDbContext friend)
    {
        _context = context;
        _friends = friend;
    }

    public async Task<List<Post>> GetAllPosts()
    {
        return await _context.Posts.ToListAsync();
    }

     public async Task<Post> CreatePostAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();

        return post;
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        return await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task<List<Post>> GetMyPostsAsync(int userId)
    {
        return await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<List<Post>> GetFriendsPostsAsync(int userId)
    {
        var friendIds = await _friends.Friendships
            .Where(f => f.UserId == userId || f.FriendId == userId)
            .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
            .ToListAsync();

        friendIds.Add(userId);

        var posts = await _context.Posts
            .Where(p => friendIds.Contains(p.UserId))
            .ToListAsync();

        return posts;
    }
    public async Task DeletePostAsync(Post post)
    {
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }
}
