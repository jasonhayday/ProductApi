using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.Services.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _service = new ProductService(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Description = "Desc1", Price = 10 },
                new Product { Id = 2, Name = "Product2", Description = "Desc2", Price = 20 }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Product1", result[0].Name);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndReturnProduct()
        {
            // Arrange
            var product = new Product { Name = "NewProduct", Description = "NewDesc", Price = 30 };

            // Act
            var result = await _service.CreateAsync(product);

            // Assert
            _repoMock.Verify(r => r.AddAsync(product), Times.Once);
            Assert.Equal("NewProduct", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ProductExists_ShouldUpdateAndReturnProduct()
        {
            // Arrange
            var existing = new Product { Id = 1, Name = "Old", Description = "OldDesc", Price = 10 };
            var updated = new Product { Name = "Updated", Description = "UpdatedDesc", Price = 50 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

            // Act
            var result = await _service.UpdateAsync(1, updated);

            // Assert
            _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
            Assert.Equal("Updated", result!.Name);
            Assert.Equal(50, result.Price);
        }

        [Fact]
        public async Task UpdateAsync_ProductDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.UpdateAsync(1, new Product());

            // Assert
            Assert.Null(result);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ProductExists_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "ToDelete" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(product), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ProductDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result);
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
        }
    }
}