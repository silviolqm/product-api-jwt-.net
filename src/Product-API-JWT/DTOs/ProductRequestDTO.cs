using System;
using System.ComponentModel.DataAnnotations;
using Product_API_JWT.Model;

namespace Product_API_JWT.DTOs;

public class ProductRequestDTO
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters.")]
    public required string Name { get; set; }

    [StringLength(300, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 300 characters.")]
    public string Description { get; set; } = string.Empty;

    [StringLength(300, MinimumLength = 10, ErrorMessage = "ImageUrl must be between 10 and 300 characters.")]
    public string ImageUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    public Product toProduct()
    {
        return new Product
        {
            Name = Name,
            Description = Description,
            ImageUrl = ImageUrl,
            Price = Price
        };
    }
}
