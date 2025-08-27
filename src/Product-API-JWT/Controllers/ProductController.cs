using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product_API_JWT.DTOs;
using Product_API_JWT.Model;
using Product_API_JWT.Services;

namespace Product_API_JWT.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController(IProductService _productService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductRequestDTO productDto)
        {
            Product newProduct = await _productService.CreateProduct(productDto.toProduct());
            ProductResponseDTO responseDto = ProductResponseDTO.FromProduct(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = responseDto.Id }, responseDto);
        }

        [HttpGet("/all")]
        public async Task<IActionResult> GetAllProducts()
        {
            List<Product> products = await _productService.GetAllProducts();
            var responseDtos = products.Select(x => ProductResponseDTO.FromProduct(x)).ToList();
            return Ok(responseDtos);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(string? searchTerm, string? sortColumn, string? sortOrder, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }
            List<Product> products = await _productService.GetProducts(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            var responseDtos = products.Select(x => ProductResponseDTO.FromProduct(x)).ToList();
            return Ok(responseDtos);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            Product product = await _productService.GetProductById(id);
            var responseDto = ProductResponseDTO.FromProduct(product);
            return Ok(responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductRequestDTO productDto)
        {
            Product updatedProduct = await _productService.UpdateProduct(id, productDto.toProduct());
            var responseDto = ProductResponseDTO.FromProduct(updatedProduct);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }
    }
}
