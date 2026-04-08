using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProductApi.API.Middleware;
using Xunit;

namespace ProductApi.API.Middleware.Tests
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task Invoke_ShouldCallNext_WhenNoException()
        {
            // Arrange
            var called = false;
            RequestDelegate next = (HttpContext ctx) =>
            {
                called = true;
                return Task.CompletedTask;
            };

            var middleware = new ExceptionMiddleware(next);
            var context = new DefaultHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.True(called); // Ensure the next delegate was called
            Assert.Equal(200, context.Response.StatusCode); // Default status code
        }

        [Fact]
        public async Task Invoke_ShouldReturn500AndMessage_WhenExceptionThrown()
        {
            // Arrange
            var exceptionMessage = "Something went wrong";
            RequestDelegate next = (HttpContext ctx) => throw new Exception(exceptionMessage);

            var middleware = new ExceptionMiddleware(next);
            var context = new DefaultHttpContext();

            // Use a MemoryStream to capture the response
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal(500, context.Response.StatusCode);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var json = JsonSerializer.Deserialize<JsonElement>(responseBody);
            Assert.Equal(exceptionMessage, json.GetProperty("message").GetString());
        }
    }
}