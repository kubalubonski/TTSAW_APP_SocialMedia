using Microsoft.EntityFrameworkCore;
using PostApi.Models;

namespace PostApi.Data;

public class PostApiDbContext : DbContext
{
    public PostApiDbContext(DbContextOptions<PostApiDbContext> options) : base(options) { }
    public DbSet<Post> Posts { get; set; }
}