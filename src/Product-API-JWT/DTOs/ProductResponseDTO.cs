using System;
using System.ComponentModel.DataAnnotations;
using Product_API_JWT.Model;

namespace Product_API_JWT.DTOs;

public class ProductResponseDTO
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters.")]
    public required string Name { get; set; }

    [StringLength(300, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 300 characters.")]
    public string Description { get; set; } = string.Empty;

    [StringLength(300, MinimumLength = 10, ErrorMessage = "ImageUrl must be between 10 and 300 characters.")]
    public string ImageUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public int Price { get; set; }

    public static ProductResponseDTO FromProduct(Product product)
    {
        return new ProductResponseDTO
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }
}
