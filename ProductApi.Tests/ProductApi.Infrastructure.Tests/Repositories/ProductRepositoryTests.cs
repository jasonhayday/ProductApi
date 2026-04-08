using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using Xunit;

namespace ProductApi.Infrastructure.Repositories.Tests
{
    public class ProductRepositoryTests
    {
        private async Task<ProductRepository> GetRepositoryAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDbContext(options);

            context.Products.AddRange(new List<Product>
        {
            new Product { Name = "Product1", Description = "Desc1", Price = 10 },
            new Product { Name = "Product2", Description = "Desc2", Price = 20 }
        });

            await context.SaveChangesAsync();

            return new ProductRepository(context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            var repo = await GetRepositoryAsync();

            var products = await repo.GetAllAsync();

            Assert.Equal(2, products.Count);
            Assert.Contains(products, p => p.Name == "Product1");
            Assert.Contains(products, p => p.Name == "Product2");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectProduct()
        {
            var repo = await GetRepositoryAsync();

            var allProducts = await repo.GetAllAsync();
            var product = await repo.GetByIdAsync(allProducts[0].Id);

            Assert.NotNull(product);
            Assert.Equal(allProducts[0].Name, product.Name);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            var repo = await GetRepositoryAsync();

            var newProduct = new Product { Name = "Product3", Description = "Desc3", Price = 30 };
            await repo.AddAsync(newProduct);

            var products = await repo.GetAllAsync();
            Assert.Equal(3, products.Count);
            Assert.Contains(products, p => p.Name == "Product3");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            var repo = await GetRepositoryAsync();

            var product = (await repo.GetAllAsync()).First();
            product.Price = 99;
            await repo.UpdateAsync(product);

            var updated = await repo.GetByIdAsync(product.Id);
            Assert.Equal(99, updated.Price);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            var repo = await GetRepositoryAsync();

            var product = (await repo.GetAllAsync()).First();
            await repo.DeleteAsync(product);

            var products = await repo.GetAllAsync();
            Assert.Single(products);
            Assert.DoesNotContain(products, p => p.Id == product.Id);
        }
    }
}