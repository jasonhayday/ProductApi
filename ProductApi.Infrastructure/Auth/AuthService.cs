namespace ProductApi.Infrastructure.Auth;

using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwt;

    public AuthService(AppDbContext context, IJwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    public async Task RegisterAsync(string username, string password)
    {
        var user = new User
        {
            Username = username,
            Password = password
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == username && x.Password == password);

        if (user == null) return null;

        return _jwt.Generate(user);
    }
}