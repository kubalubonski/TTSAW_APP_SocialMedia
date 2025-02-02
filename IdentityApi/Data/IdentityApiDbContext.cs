using IdentityApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Data
{
    public class IdentityApiDbContext : DbContext
    {
        public IdentityApiDbContext(DbContextOptions<IdentityApiDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}