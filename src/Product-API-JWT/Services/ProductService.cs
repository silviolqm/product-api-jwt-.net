using Product_API_JWT.Data;
using Product_API_JWT.Exceptions;
using Product_API_JWT.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Product_API_JWT.DTOs.Pagination;

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

    public async Task<PaginatedList<Product>> GetProducts(string? searchTerm, string? sortColumn, string? sortOrder, int pageNumber, int pageSize)
    {
        var query = _dbContext.Products.AsNoTracking();

        //SearchTerm
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x => x.Name.Contains(searchTerm) || x.Description.Contains(searchTerm));
        }

        //SortColumn
        Expression<Func<Product, object>> columnSelector = sortColumn?.ToLower() switch
        {
            "name" => product => product.Name,
            "price" => product => product.Price,
            _ => product => product.Id,
        };

        //SortOrder
        if (!string.IsNullOrWhiteSpace(sortOrder))
        {
            if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(columnSelector);
            }
        }
        else
        {
            query = query.OrderBy(columnSelector);
        }

        var totalCount = await query.CountAsync();

        //Pagination
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<Product>(products, totalCount, pageNumber, pageSize);
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
