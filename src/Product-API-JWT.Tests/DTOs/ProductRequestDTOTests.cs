using System;
using FluentAssertions;
using Product_API_JWT.DTOs;
using Product_API_JWT.Model;

namespace Product_API_JWT.Tests.DTOs;

public class ProductRequestDTOTests
{
    [Fact]
    public void ToProduct_Should_Return_Product()
    {
        ProductRequestDTO productRequestDTO = new ProductRequestDTO
        {
            Name = "product name",
            Description = "product description",
            ImageUrl = "product img url",
            Price = 10.5m
        };

        Product convertedProduct = productRequestDTO.toProduct();

        convertedProduct.Name.Should().Be(productRequestDTO.Name);
        convertedProduct.Description.Should().Be(productRequestDTO.Description);
        convertedProduct.ImageUrl.Should().Be(productRequestDTO.ImageUrl);
        convertedProduct.Price.Should().Be(productRequestDTO.Price);
    }
}