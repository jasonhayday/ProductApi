using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using ProductApi.API.Controllers;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using Xunit;

namespace ProductApi.API.Controllers.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _serviceMock;
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _serviceMock = new Mock<IProductService>();
            _cacheMock = new Mock<IDistributedCache>();
            _controller = new ProductController(_serviceMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnFromCache_WhenCacheExists()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100 },
                new Product { Id = 2, Name = "Product 2", Price = 200 }
            };

            var cachedBytes = JsonSerializer.SerializeToUtf8Bytes(products);

            _cacheMock
                .Setup(c => c.GetAsync("products", It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedBytes);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
            _serviceMock.Verify(s => s.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAll_ShouldReturnFromService_WhenCacheEmpty()
        {
            // Arrange
            _cacheMock
                .Setup(c => c.GetAsync("products", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100 },
                new Product { Id = 2, Name = "Product 2", Price = 200 }
            };

            _serviceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
            _serviceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldCallService_AndClearCache()
        {
            // Arrange
            var product = new Product { Name = "New", Price = 50 };
            _serviceMock.Setup(s => s.CreateAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _controller.Create(product);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, okResult.Value);
            _serviceMock.Verify(s => s.CreateAsync(product), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync("products", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            var product = new Product { Name = "Updated", Price = 100 };
            _serviceMock.Setup(s => s.UpdateAsync(1, product)).ReturnsAsync(product);

            // Act
            var result = await _controller.Update(1, product);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, okResult.Value);
            _cacheMock.Verify(c => c.RemoveAsync("products", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var product = new Product { Name = "Missing", Price = 100 };
            _serviceMock.Setup(s => s.UpdateAsync(1, product)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.Update(1, product);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _cacheMock.Verify(c => c.RemoveAsync("products", It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenProductDeleted()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<OkResult>(result);
            _cacheMock.Verify(c => c.RemoveAsync("products", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenProductNotDeleted()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _cacheMock.Verify(c => c.RemoveAsync("products", It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Search_ShouldFilterByNameAndPrice()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Apple", Price = 10 },
                new Product { Name = "Banana", Price = 20 },
                new Product { Name = "Cherry", Price = 30 }
            };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.Search("a", 15, 25);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var filtered = Assert.IsType<List<Product>>(okResult.Value);
            Assert.Single(filtered);
            Assert.Equal("Banana", filtered[0].Name);
        }
    }
}