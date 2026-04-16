using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Services.Interfaces;
using ZeroTrustAPI.Api.Services.Implementations;
using ZeroTrustAPI.Api.Repositories.Interfaces;
using ZeroTrustAPI.Api.Repositories.Implementations;
using ZeroTrustAPI.Api.Security;
using ZeroTrustAPI.Api.Health;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Ajout de GSSEncryptionMode=Disable (évite la librairie Kerberos)
var originalConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(originalConnection) && !originalConnection.Contains("GSSEncryptionMode"))
{
    var separator = originalConnection.Contains(";") ? "" : ";";
    var modifiedConnection = $"{originalConnection}{separator}GSSEncryptionMode=Disable";
    builder.Configuration["ConnectionStrings:DefaultConnection"] = modifiedConnection;
}

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing")))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// DbContext
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

// Health check – utilise votre PostgresHealthCheck
builder.Services.AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("postgres", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);

// Vos services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IHealthService, HealthService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

var app = builder.Build();

// Migrations (sauf en mode TESTING)
if (Environment.GetEnvironmentVariable("TESTING") != "true")
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erreur lors de l'application des migrations");
        // L'API continue – le health check signalera l'état de la base
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();