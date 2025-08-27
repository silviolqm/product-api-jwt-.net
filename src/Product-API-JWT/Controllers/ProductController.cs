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
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

        [HttpGet("/all")]
        public async Task<IActionResult> GetAllProducts()
        {
            List<Product> products = await _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsPaginated(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }
            List<Product> products = await _productService.GetAllProductsPaginated(pageNumber, pageSize);
            return Ok(products);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            Product product = await _productService.GetProductById(id);
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductRequestDTO productDto)
        {
            Product updatedProduct = await _productService.UpdateProduct(id, productDto.toProduct());
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }
    }
}
