using Product_API_JWT.Model;

namespace Product_API_JWT.Services;

public interface IProductService
{
    Task<Product> CreateProduct(Product product);
    Task<List<Product>> GetAllProducts();
    Task<List<Product>> GetProductsPaginated(int pageNumber, int pageSize);
    Task<Product> GetProductById(int id);
    Task<Product> UpdateProduct(int id, Product product);
    Task DeleteProduct(int id);
}
