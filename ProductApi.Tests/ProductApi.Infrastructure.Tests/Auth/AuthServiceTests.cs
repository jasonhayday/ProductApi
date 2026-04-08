using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Auth;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Auth.Tests
{
    public class AuthServiceTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IJwtService> _jwtMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTestDb")
                .Options;

            _context = new AppDbContext(options);

            // Act
            _jwtMock = new Mock<IJwtService>();

            // Assert
            _authService = new AuthService(_context, _jwtMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var username = "testuser";
            var password = "pass123";

            // Act
            await _authService.RegisterAsync(username, password);

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            Assert.NotNull(user);
            Assert.Equal(password, user!.Password);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var username = "loginuser";
            var password = "loginpass";

            var user = new User { Username = username, Password = password };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _jwtMock.Setup(j => j.Generate(It.Is<User>(u => u.Username == username)))
                .Returns("mocktoken");

            // Act
            var token = await _authService.LoginAsync(username, password);

            // Assert
            Assert.Equal("mocktoken", token);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ShouldReturnNull()
        {
            // Act
            var token = await _authService.LoginAsync("wronguser", "wrongpass");

            // Assert
            Assert.Null(token);
        }
    }
}