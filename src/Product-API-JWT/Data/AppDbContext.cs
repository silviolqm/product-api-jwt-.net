using System;
using Microsoft.EntityFrameworkCore;
using Product_API_JWT.Model;

namespace Product_API_JWT.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Product> Products { get; set; }
}
