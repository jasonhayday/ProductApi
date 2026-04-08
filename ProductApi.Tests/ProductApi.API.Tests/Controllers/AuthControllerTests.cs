using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.API.Controllers;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using Xunit;

namespace ProductApi.API.Controllers.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_ShouldCallRegisterAsync_AndReturnOk()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "password123" };

            // Act
            var result = await _controller.Register(user);

            // Assert
            _mockAuthService.Verify(a => a.RegisterAsync(user.Username, user.Password), Times.Once);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WithToken_WhenLoginSucceeds()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "password123" };
            var fakeToken = "fake-jwt-token";

            _mockAuthService.Setup(a => a.LoginAsync(user.Username, user.Password))
                .ReturnsAsync(fakeToken);

            // Act
            var result = await _controller.Login(user);

            // Assert
            _mockAuthService.Verify(a => a.LoginAsync(user.Username, user.Password), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var tokenProperty = okResult.Value!.GetType().GetProperty("token");
            Assert.NotNull(tokenProperty);
            var tokenValue = tokenProperty!.GetValue(okResult.Value) as string;
            Assert.Equal(fakeToken, tokenValue);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenLoginFails()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "wrongpassword" };

            _mockAuthService.Setup(a => a.LoginAsync(user.Username, user.Password))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _controller.Login(user);

            // Assert
            _mockAuthService.Verify(a => a.LoginAsync(user.Username, user.Password), Times.Once);
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }
    }
}