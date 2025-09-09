using System;
using FluentAssertions;
using Product_API_JWT.DTOs;
using Product_API_JWT.Model;

namespace Product_API_JWT.Tests.DTOs;

public class ProductResponseDTOTests
{
    [Fact]
    public void FromProduct_Should_Return_ProductResponseDTO()
    {
        Product product = new Product
        {
            Name = "product name",
            Description = "product description",
            ImageUrl = "product img url",
            Price = 10.5m
        };

        ProductResponseDTO responseDTO = ProductResponseDTO.FromProduct(product);

        responseDTO.Name.Should().Be(product.Name);
        responseDTO.Description.Should().Be(product.Description);
        responseDTO.ImageUrl.Should().Be(product.ImageUrl);
        responseDTO.Price.Should().Be(product.Price);
    }
}
