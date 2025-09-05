using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Product_API_JWT.Model;

namespace Product_API_JWT.Data;

public static class DatabaseConfig
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
    }

    public static void DoMigrations(this IApplicationBuilder app, bool isProduction)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var appliedMigrations = context.Database.GetAppliedMigrations();
        if (!appliedMigrations.Any())
        {
            context.Database.Migrate();
        }
        else if (isProduction)
        {
            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                context.Database.Migrate();
            }
        }
    }

    public static void SeedDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var numberOfProductsToSeed = 1000;

                if (!dbContext.Products.Any())
                {
                    var products = Enumerable.Range(1, numberOfProductsToSeed).Select(i => new Product
                    {
                        Name = $"Product {i}",
                        Description = $"Description for Product {i}",
                        ImageUrl = $"https://example.com/images/product{i}.jpg",
                        Price = i * 10
                    });
                    dbContext.Products.AddRange(products);
                    dbContext.SaveChanges();
                }
            }
        }
}
