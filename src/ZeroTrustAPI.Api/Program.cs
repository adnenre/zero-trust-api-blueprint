using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Services.Interfaces;
using ZeroTrustAPI.Api.Services.Implementations;
using ZeroTrustAPI.Api.Repositories.Interfaces;
using ZeroTrustAPI.Api.Repositories.Implementations;
using ZeroTrustAPI.Api.Security;
using ZeroTrustAPI.Api.Health;
using HealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;
using Microsoft.Extensions.DependencyInjection;   // for AddDbContext, AddScoped, etc.
using Npgsql.EntityFrameworkCore.PostgreSQL;      // for UseNpgsql extension

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

if (Environment.GetEnvironmentVariable("TESTING") == "true")
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("postgres", failureStatus: HealthStatus.Unhealthy);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IHealthService, HealthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (Environment.GetEnvironmentVariable("TESTING") != "true")
    {
        dbContext.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();