using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using Xunit;

namespace ProductApi.Infrastructure.Data.Tests
{
    public class AppDbContextTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void CanInstantiateDbContext()
        {
            // Act
            using var context = GetInMemoryDbContext();

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Products);
            Assert.NotNull(context.Users);
        }

        [Fact]
        public void CanAddAndRetrieveProduct()
        {
            using var context = GetInMemoryDbContext();

            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100
            };

            // Act
            context.Products.Add(product);
            context.SaveChanges();

            // Assert
            var savedProduct = context.Products.FirstOrDefault(p => p.Name == "Test Product");
            Assert.NotNull(savedProduct);
            Assert.Equal("Test Description", savedProduct.Description);
            Assert.Equal(100, savedProduct.Price);
        }

        [Fact]
        public void CanAddAndRetrieveUser()
        {
            using var context = GetInMemoryDbContext();

            var user = new User
            {
                Username = "testuser",
                Password = "hashedpassword"
            };

            // Act
            context.Users.Add(user);
            context.SaveChanges();

            // Assert
            var savedUser = context.Users.FirstOrDefault(u => u.Username == "testuser");
            Assert.NotNull(savedUser);
            Assert.Equal("hashedpassword", savedUser.Password);
        }
    }
}