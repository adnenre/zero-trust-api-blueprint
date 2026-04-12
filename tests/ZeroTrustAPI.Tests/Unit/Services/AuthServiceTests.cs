using Moq;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Repositories.Interfaces;
using ZeroTrustAPI.Api.Security;
using ZeroTrustAPI.Api.Services.Implementations;
using Xunit;

namespace ZeroTrustAPI.Tests.Unit.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var user = new User { Id = 1, Username = "john", PasswordHash = "hashed123" };
        mockRepo.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.Verify("pass", "hashed123")).Returns(true);

        var service = new AuthService(mockRepo.Object, mockHasher.Object);

        // Act
        var result = await service.AuthenticateAsync("john", "pass");

        // Assert
        Assert.True(result.Success);
        Assert.Equal("john", result.User?.Username);
        Assert.Equal("Login successful", result.Message);
        mockRepo.Verify(r => r.GetByUsernameAsync("john"), Times.Once);
        mockHasher.Verify(h => h.Verify("pass", "hashed123"), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User?)null);

        var mockHasher = new Mock<IPasswordHasher>();
        var service = new AuthService(mockRepo.Object, mockHasher.Object);

        // Act
        var result = await service.AuthenticateAsync("unknown", "any");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found", result.Message);
        Assert.Null(result.User);
        mockRepo.Verify(r => r.GetByUsernameAsync("unknown"), Times.Once);
        mockHasher.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_WrongPassword_ReturnsFailure()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var user = new User { Id = 1, Username = "john", PasswordHash = "hashed123" };
        mockRepo.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.Verify("wrong", "hashed123")).Returns(false);

        var service = new AuthService(mockRepo.Object, mockHasher.Object);

        // Act
        var result = await service.AuthenticateAsync("john", "wrong");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid password", result.Message);
        Assert.Null(result.User);
        mockRepo.Verify(r => r.GetByUsernameAsync("john"), Times.Once);
        mockHasher.Verify(h => h.Verify("wrong", "hashed123"), Times.Once);
    }
}