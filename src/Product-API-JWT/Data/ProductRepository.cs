using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Product_API_JWT.DTOs;
using Product_API_JWT.Exceptions;
using Product_API_JWT.Model;

namespace Product_API_JWT.Data;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public async Task<ProductResponseDTO> CreateProduct(ProductRequestDTO productDto)
    {
        Product newProduct = productDto.toProduct();
        await dbContext.AddAsync(newProduct);
        await dbContext.SaveChangesAsync();
        return ProductResponseDTO.FromProduct(newProduct);
    }

    public async Task DeleteProduct(int id)
    {
        await dbContext.Products
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<List<ProductResponseDTO>> GetAllProducts()
    {
        return await dbContext.Products
            .Select(x => ProductResponseDTO.FromProduct(x))
            .ToListAsync();
    }

    public async Task<ProductResponseDTO> GetProductById(int id)
    {
        return await dbContext.Products
            .Where(x => x.Id == id)
            .Select(x => ProductResponseDTO.FromProduct(x))
            .FirstOrDefaultAsync() ?? throw new ProductNotFoundException($"Product with id {id} not found.");
    }

    public async Task<ProductResponseDTO> UpdateProduct(int id, ProductRequestDTO productDto)
    {
        Product productToUpdate = await dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new ProductNotFoundException($"Product with id {id} not found.");

        productToUpdate.Name = productDto.Name;
        productToUpdate.Description = productDto.Description;
        productToUpdate.Price = productDto.Price;

        await dbContext.SaveChangesAsync();

        return ProductResponseDTO.FromProduct(productToUpdate);
    }
}