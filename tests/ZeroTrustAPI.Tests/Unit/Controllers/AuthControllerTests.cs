using Microsoft.AspNetCore.Mvc;
using Moq;
using ZeroTrustAPI.Api.Controllers;
using ZeroTrustAPI.Api.Services.Interfaces;
using ZeroTrustAPI.Api.DTOs;
using Xunit;

namespace ZeroTrustAPI.Tests.Unit.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithAuthResult()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        var expectedResult = new AuthResult(true, new UserDto(1, "testuser"), "OK");
        mockService.Setup(s => s.AuthenticateAsync("testuser", "password123"))
                   .ReturnsAsync(expectedResult);
        var controller = new AuthController(mockService.Object);
        var request = new LoginRequest("testuser", "password123");

        // Act
        var result = await controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<AuthResult>(okResult.Value);
        Assert.True(returnedResult.Success);
        mockService.Verify(s => s.AuthenticateAsync("testuser", "password123"), Times.Once);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        mockService.Setup(s => s.AuthenticateAsync("wrong", "wrong"))
                   .ReturnsAsync(new AuthResult(false, null, "Invalid"));
        var controller = new AuthController(mockService.Object);
        var request = new LoginRequest("wrong", "wrong");

        // Act
        var result = await controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
        mockService.Verify(s => s.AuthenticateAsync("wrong", "wrong"), Times.Once);
    }
}