using System;
using System.ComponentModel.DataAnnotations;
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
        var dto = new ProductRequestDTO
        {
            Name = "Valid Product",
            Description = "This is a valid description.",
            ImageUrl = "http://example.com/image.jpg",
            Price = 99.99m
        };

        var results = ValidateModel(dto);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Missing()
    {
        var dto = new ProductRequestDTO
        {
            Name = null!,
            Description = "This is a valid description.",
            ImageUrl = "http://example.com/image.jpg",
            Price = 99.99m
        };

        var results = ValidateModel(dto);

        results.Should().Contain(v => v.MemberNames.Contains("Name") && v.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void Should_Fail_When_Price_Is_Zero()
    {
        var dto = new ProductRequestDTO
        {
            Name = "Valid Product",
            Description = "This is a valid description.",
            ImageUrl = "http://example.com/image.jpg",
            Price = 0m
        };

        var results = ValidateModel(dto);

        results.Should().Contain(v => v.MemberNames.Contains("Price") && v.ErrorMessage!.Contains("greater than zero"));
    }
}