namespace ProductApi.Infrastructure.Data;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
}