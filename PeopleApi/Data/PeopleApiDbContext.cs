namespace PeopleApi.Data;

using Microsoft.EntityFrameworkCore;
using PeopleApi.Models;

public class PeopleApiDbContext : DbContext
{
    public PeopleApiDbContext(DbContextOptions<PeopleApiDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Friendship> Friendships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.Friend)
            .WithMany()
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<User>().ToTable("Users", t => t.ExcludeFromMigrations());
    }
}