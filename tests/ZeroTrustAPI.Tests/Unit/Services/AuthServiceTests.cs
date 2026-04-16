using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Services.Implementations;
using ZeroTrustAPI.Tests.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroTrustAPI.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        var inMemorySettings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "superSecretKeyAtLeast32CharsLong123!",
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience"
        };
        _config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        _authService = new AuthService(_context, _config);
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsSuccess()
    {
        var request = TestData.GetValidRegisterRequest();
        var result = await _authService.RegisterAsync(request);
        Assert.True(result.Success);
        Assert.NotNull(result.UserId);
    }

    [Fact]
    public async Task Register_DuplicateUsername_ReturnsFailure()
    {
        var user = TestData.GetValidUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = TestData.GetValidRegisterRequest();
        request.Username = user.Username;
        var result = await _authService.RegisterAsync(request);
        Assert.False(result.Success);
        // Fixed: added trailing period to match actual error message
        Assert.Contains("Username or email already exists.", result.Errors ?? System.Array.Empty<string>());
    }

    [Fact]
    public async Task Register_PasswordMismatch_ReturnsFailure()
    {
        var request = TestData.GetValidRegisterRequest();
        request.ConfirmPassword = "different";
        var result = await _authService.RegisterAsync(request);
        Assert.False(result.Success);
        // Fixed: added trailing period
        Assert.Contains("Passwords do not match.", result.Errors ?? System.Array.Empty<string>());
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        var user = TestData.GetValidUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new LoginRequest { Username = user.Username, Password = "Test123!" };
        var result = await _authService.LoginAsync(request);
        Assert.True(result.Success);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsFailure()
    {
        var user = TestData.GetValidUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new LoginRequest { Username = user.Username, Password = "wrong" };
        var result = await _authService.LoginAsync(request);
        Assert.False(result.Success);
        // Fixed: added trailing period
        Assert.Contains("Invalid username or password.", result.Errors ?? System.Array.Empty<string>());
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsNewTokens()
    {
        var user = TestData.GetValidUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var loginResult = await _authService.LoginAsync(new LoginRequest { Username = user.Username, Password = "Test123!" });

        var refreshResult = await _authService.RefreshTokenAsync(loginResult.RefreshToken!);
        Assert.True(refreshResult.Success);
        Assert.NotEqual(loginResult.AccessToken, refreshResult.AccessToken);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ReturnsFailure()
    {
        var result = await _authService.RefreshTokenAsync("invalid-token");
        Assert.False(result.Success);
        // Fixed: added trailing period
        Assert.Contains("Invalid or expired refresh token.", result.Errors ?? System.Array.Empty<string>());
    }

    [Fact]
    public async Task RevokeRefreshTokensAsync_RevokesAll()
    {
        var user = TestData.GetValidUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _authService.LoginAsync(new LoginRequest { Username = user.Username, Password = "Test123!" });
        await _authService.RevokeRefreshTokensAsync(user.Id);

        var tokens = await _context.RefreshTokens.Where(rt => rt.UserId == user.Id && !rt.IsRevoked).ToListAsync();
        Assert.Empty(tokens);
    }
}