using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using ProductApi.Domain.Entities;

namespace ProductApi.Domain.Entities.Tests
{
    public class ProductTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void Product_DefaultCreatedAt_ShouldBeNow()
        {
            // Arrange & Act
            var product = new Product { Name = "Test", Description = "Desc", Price = 10 };

            // Assert
            var now = DateTime.Now;
            Assert.True((now - product.CreatedAt).TotalSeconds < 1);
        }

        [Fact]
        public void Product_WithRequiredName_ValidatesSuccessfully()
        {
            // Arrange
            var product = new Product { Name = "ValidName", Description = "Desc", Price = 10 };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Product_WithoutName_ShouldFailValidation()
        {
            // Arrange
            var product = new Product { Name = null!, Description = "Desc", Price = 10 };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Product_PriceNegative_ShouldFailValidation()
        {
            // Arrange
            var product = new Product { Name = "Test", Description = "Desc", Price = -5 };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Price"));
        }

        [Fact]
        public void Product_CanSetAllProperties()
        {
            // Arrange & Act
            var createdAt = new DateTime(2020, 1, 1);
            var product = new Product
            {
                Id = 1,
                Name = "Test",
                Description = "Description",
                Price = 100,
                CreatedAt = createdAt
            };

            // Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("Test", product.Name);
            Assert.Equal("Description", product.Description);
            Assert.Equal(100, product.Price);
            Assert.Equal(createdAt, product.CreatedAt);
        }
    }
}