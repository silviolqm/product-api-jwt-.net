using System;
using Product_API_JWT.Services;

namespace Product_API_JWT.Configs;

public static class DependencyInjectionConfig
{
    public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
}
