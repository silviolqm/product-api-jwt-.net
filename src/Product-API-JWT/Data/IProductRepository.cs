using System;
using System.Runtime.CompilerServices;
using Product_API_JWT.DTOs;

namespace Product_API_JWT.Data;

public interface IProductRepository
{
    Task<ProductResponseDTO> CreateProduct(ProductRequestDTO productDto);
    Task<List<ProductResponseDTO>> GetAllProducts();
    Task<ProductResponseDTO> GetProductById(int id);
    Task<ProductResponseDTO> UpdateProduct(int id, ProductRequestDTO productDto);
    Task DeleteProduct(int id);
}
