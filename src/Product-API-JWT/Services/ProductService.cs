using Product_API_JWT.Data;
using Product_API_JWT.Exceptions;
using Product_API_JWT.Model;
using Microsoft.EntityFrameworkCore;

namespace Product_API_JWT.Services;

public class ProductService(AppDbContext _dbContext) : IProductService
{
    public async Task<Product> CreateProduct(Product product)
    {
        await _dbContext.AddAsync(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }

    public async Task DeleteProduct(int id)
    {
        await _dbContext.Products
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _dbContext.Products.ToListAsync();
    }

    public async Task<Product> GetProductById(int id)
    {
        return await _dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new ProductNotFoundException($"Product with id {id} not found.");
    }

    public async Task<Product> UpdateProduct(int id, Product product)
    {
        Product productToUpdate = await _dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new ProductNotFoundException($"Product with id {id} not found.");

        productToUpdate.Name = product.Name;
        productToUpdate.Description = product.Description;
        productToUpdate.Price = product.Price;

        await _dbContext.SaveChangesAsync();

        return productToUpdate;
    }
}
