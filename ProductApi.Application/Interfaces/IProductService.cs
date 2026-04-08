using ProductApi.Domain.Entities;

namespace ProductApi.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> CreateAsync(Product p);
        Task<Product?> UpdateAsync(int id, Product p);
        Task<bool> DeleteAsync(int id);
    }
}