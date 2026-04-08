namespace ProductApi.API.Controllers;

using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IDistributedCache _cache;

    public ProductController(IProductService service, IDistributedCache cache)
    {
        _service = service;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cacheKey = "products";

        var cached = await _cache.GetStringAsync(cacheKey);

        if (cached != null)
            return Ok(JsonSerializer.Deserialize<List<Product>>(cached));

        var data = await _service.GetAllAsync();

        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(data),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product p)
    {
        var result = await _service.CreateAsync(p);

        await _cache.RemoveAsync("products");

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product p)
    {
        var result = await _service.UpdateAsync(id, p);

        if (result == null) return NotFound();

        await _cache.RemoveAsync("products");

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);

        if (!success) return NotFound();

        await _cache.RemoveAsync("products");

        return Ok();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string? name, decimal? minPrice, decimal? maxPrice)
    {
        var data = await _service.GetAllAsync();

        var result = data.AsQueryable();

        if (!string.IsNullOrEmpty(name))
            result = result.Where(x => x.Name.ToLower().Contains(name.ToLower()));

        if (minPrice.HasValue)
            result = result.Where(x => x.Price >= minPrice);

        if (maxPrice.HasValue)
            result = result.Where(x => x.Price <= maxPrice);

        return Ok(result.ToList());
    }
}