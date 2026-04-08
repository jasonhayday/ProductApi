using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Auth;
using Xunit;

namespace ProductApi.Infrastructure.Auth.Tests
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public JwtServiceTests()
        {
            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string>
            {
                {"Jwt:Key", "ThisIsASecretKeyForTesting12345!"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _jwtService = new JwtService(_configuration);
        }

        [Fact]
        public void Generate_ShouldReturnValidJwtToken()
        {
            // Arrange
            var user = new User { Username = "testuser" };

            // Act
            var token = _jwtService.Generate(user);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == "testuser");
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        }
    }
}