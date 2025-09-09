using System;
using System.ComponentModel.DataAnnotations;
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

    private IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void Should_Succeed_With_Valid_Data()
    {
        var dto = new ProductResponseDTO
        {
            Id = 1,
            Name = "Valid Product",
            Description = "This is a valid description.",
            ImageUrl = "http://example.com/image.jpg",
            Price = 99.99m
        };

        var results = ValidateModel(dto);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Null()
    {
        var dto = new ProductResponseDTO
        {
            Id = 1,
            Name = null,
            Description = "This is a valid description.",
            ImageUrl = "http://example.com/image.jpg",
            Price = 99.99m
        };

        var results = ValidateModel(dto);

        results.Should().Contain(v => v.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Should_Fail_When_Price_Is_Zero()
    {
        var dto = new ProductResponseDTO
        {
            Id = 1,
            Name = "Valid Product",
            Description = "This is a valid description.",
            ImageUrl = "http://example.com/image.jpg",
            Price = 0m
        };

        var results = ValidateModel(dto);

        results.Should().Contain(v => v.MemberNames.Contains("Price"));
    }
}
