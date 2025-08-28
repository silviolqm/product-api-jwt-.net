using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product_API_JWT.DTOs;
using Product_API_JWT.DTOs.Pagination;
using Product_API_JWT.Model;
using Product_API_JWT.Services;

namespace Product_API_JWT.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController(IProductService _productService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateProduct(ProductRequestDTO productDto)
        {
            Product newProduct = await _productService.CreateProduct(productDto.toProduct());
            ProductResponseDTO responseDto = ProductResponseDTO.FromProduct(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = responseDto.Id }, responseDto);
        }

        [HttpGet("/all")]
        public async Task<ActionResult<ProductResponseDTO>> GetAllProducts()
        {
            List<Product> products = await _productService.GetAllProducts();
            var responseDtos = products.Select(x => ProductResponseDTO.FromProduct(x)).ToList();
            return Ok(responseDtos);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<ProductResponseDTO>>> GetProducts(string? searchTerm, string? sortColumn, string? sortOrder, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }
            PaginatedList<Product> products = await _productService.GetProducts(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            var responseDtos = new PaginatedList<ProductResponseDTO>
            (
                items: products.Items.Select(x => ProductResponseDTO.FromProduct(x)).ToList(),
                count: products.TotalCount,
                pageIndex: products.PageIndex,
                pageSize: products.PageSize
            );
            return Ok(responseDtos);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<ProductResponseDTO>> GetProductById(int id)
        {
            Product product = await _productService.GetProductById(id);
            var responseDto = ProductResponseDTO.FromProduct(product);
            return Ok(responseDto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(int id, ProductRequestDTO productDto)
        {
            Product updatedProduct = await _productService.UpdateProduct(id, productDto.toProduct());
            var responseDto = ProductResponseDTO.FromProduct(updatedProduct);
            return Ok(responseDto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }
    }
}
