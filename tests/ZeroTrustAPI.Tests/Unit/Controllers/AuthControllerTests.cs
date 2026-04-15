using Microsoft.AspNetCore.Mvc;
using Moq;
using ZeroTrustAPI.Api.Controllers;
using ZeroTrustAPI.Api.Services.Interfaces;
using ZeroTrustAPI.Api.DTOs;
using Xunit;
using System.Threading.Tasks;
using System;

namespace ZeroTrustAPI.Tests.Unit.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithAuthResult()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        var expectedResult = new AuthResult
        {
            Success = true,
            AccessToken = "access_token",
            RefreshToken = "refresh_token",
            UserId = Guid.NewGuid()
        };
        var loginRequest = new LoginRequest
        {
            Username = "testuser",
            Password = "password123"
        };
        mockService.Setup(s => s.LoginAsync(loginRequest))
                   .ReturnsAsync(expectedResult);
        var controller = new AuthController(mockService.Object);

        // Act
        var result = await controller.Login(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<AuthResult>(okResult.Value);
        Assert.True(returnedResult.Success);
        mockService.Verify(s => s.LoginAsync(loginRequest), Times.Once);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        var loginRequest = new LoginRequest
        {
            Username = "wrong",
            Password = "wrong"
        };
        var expectedResult = new AuthResult
        {
            Success = false,
            Errors = new[] { "Invalid username or password." }
        };
        mockService.Setup(s => s.LoginAsync(loginRequest))
                   .ReturnsAsync(expectedResult);
        var controller = new AuthController(mockService.Object);

        // Act
        var result = await controller.Login(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        // The controller may return the Errors array directly (string[]) instead of AuthResult
        var errors = Assert.IsType<string[]>(unauthorizedResult.Value);
        Assert.Contains("Invalid username or password.", errors);
        mockService.Verify(s => s.LoginAsync(loginRequest), Times.Once);
    }
}