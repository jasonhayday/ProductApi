using ProductApi.Domain.Entities;

namespace ProductApi.Application.Interfaces
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}
