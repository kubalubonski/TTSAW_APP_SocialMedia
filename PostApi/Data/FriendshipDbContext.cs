using Microsoft.EntityFrameworkCore;
using PostApi.Models;

namespace PostApi.Data;

public class FriendshipDbContext : DbContext
{
    public FriendshipDbContext(DbContextOptions<FriendshipDbContext> options) : base(options) { }
    public DbSet<Friendship> Friendships { get; set; }
}