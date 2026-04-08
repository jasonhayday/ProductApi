namespace ProductApi.Application.Services;

using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

public class ProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Product>> GetAllAsync()
        => await _repo.GetAllAsync();

    public async Task<Product> CreateAsync(Product product)
    {
        await _repo.AddAsync(product);
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product updated)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return null;

        product.Name = updated.Name;
        product.Description = updated.Description;
        product.Price = updated.Price;

        await _repo.UpdateAsync(product);
        return product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        await _repo.DeleteAsync(product);
        return true;
    }
}