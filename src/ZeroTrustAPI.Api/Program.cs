using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Services.Interfaces;
using ZeroTrustAPI.Api.Services.Implementations;
using ZeroTrustAPI.Api.Repositories.Interfaces;
using ZeroTrustAPI.Api.Repositories.Implementations;
using ZeroTrustAPI.Api.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Conditionally register database provider
if (Environment.GetEnvironmentVariable("TESTING") == "true")
{
    // Use InMemory database for integration tests
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
else
{
    // Use real SQL Server in normal environments
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IHealthService, HealthService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();