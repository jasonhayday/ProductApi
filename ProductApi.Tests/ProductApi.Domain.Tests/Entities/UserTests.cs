using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using ProductApi.Domain.Entities;

namespace ProductApi.Domain.Entities.Tests
{
    public class UserTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void User_CanSetAllProperties()
        {
            // Arrange & Act
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Password = "password123"
            };

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("password123", user.Password);
        }

        [Fact]
        public void User_ValidUser_PassesValidation()
        {
            // Arrange
            var user = new User { Username = "user1", Password = "pass1" };

            // Act
            var results = ValidateModel(user);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void User_MissingUsername_FailsValidation()
        {
            // Arrange
            var user = new User { Username = null!, Password = "pass1" };

            // Act
            var results = ValidateModel(user);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Username"));
        }

        [Fact]
        public void User_MissingPassword_FailsValidation()
        {
            // Arrange
            var user = new User { Username = "user1", Password = null! };

            // Act
            var results = ValidateModel(user);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void User_MissingUsernameAndPassword_FailsValidation()
        {
            // Arrange
            var user = new User { Username = null!, Password = null! };

            // Act
            var results = ValidateModel(user);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Username"));
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }
    }
}