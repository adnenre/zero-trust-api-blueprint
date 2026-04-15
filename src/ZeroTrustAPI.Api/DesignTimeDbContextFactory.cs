using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ZeroTrustAPI.Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        // Dummy connection string – only used at design time
        optionsBuilder.UseNpgsql("Host=localhost;Database=DesignTime;Username=postgres;Password=postgres");

        return new AppDbContext(optionsBuilder.Options);
    }
}