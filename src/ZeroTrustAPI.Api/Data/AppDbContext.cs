using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
}