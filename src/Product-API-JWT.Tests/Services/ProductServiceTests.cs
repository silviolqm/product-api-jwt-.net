using System.Data.Common;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Product_API_JWT.Data;
using Product_API_JWT.Exceptions;
using Product_API_JWT.Model;
using Product_API_JWT.Services;


namespace Product_API_JWT.Tests.Services;

public class ProductServiceTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AppDbContext> _contextOptions;

    public ProductServiceTests()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        // Create the schema and seed some data
        using var context = new AppDbContext(_contextOptions);
        context.Database.EnsureCreated();
    }

    AppDbContext CreateContext() => new AppDbContext(_contextOptions);
    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task GetProductById_Should_Return_Product()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var product = new Product { Name = "Test Product", Price = 9.99m };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var result = await productService.GetProductById(product.Id);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(9.99m);
    }

    [Fact]
    public async Task GetAllProducts_Should_Return_All_Products()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var products = Enumerable.Range(1, 10).Select(i => new Product
        {
            Name = $"Product {i}",
            Description = $"Description for Product {i}",
            ImageUrl = $"https://example.com/images/product{i}.jpg",
            Price = i * 10
        });
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        var result = await productService.GetAllProducts();

        result.Should().NotBeNull();
        result.Should().HaveCount(10);
        result.Should().BeEquivalentTo(products, options => options.Excluding(p => p.Id));
    }

    [Fact]
    public async Task GetProducts_Should_Return_A_PaginatedList_Of_Products()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var products = Enumerable.Range(1, 25).Select(i => new Product
        {
            Name = $"Product {i}",
            Description = $"Description for Product {i}",
            ImageUrl = $"https://example.com/images/product{i}.jpg",
            Price = i * 10
        });
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        int pageNumber = 2;
        int pageSize = 5;
        var result = await productService.GetProducts(null, null, null, pageNumber, pageSize);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(25);
        result.PageIndex.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.Items.Should().HaveCount(pageSize);
        result.Items.Should().BeEquivalentTo(products.Skip(pageSize * (pageNumber - 1)).Take(pageSize), options => options.Excluding(p => p.Id));
    }

    [Fact]
    public async Task CreateProduct_Should_Create_A_Product()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var product = new Product { Name = "Test Product Creation", Price = 500m };

        var savedProduct = await productService.CreateProduct(product);

        savedProduct.Should().NotBeNull();
        savedProduct.Name.Should().Be("Test Product Creation");
        savedProduct.Price.Should().Be(500m);
    }

    [Fact]
    public async Task DeleteProduct_Should_Delete_A_Product()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var product = new Product { Name = "Test Product Deletion", Price = 10m };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        await productService.DeleteProduct(product.Id);
        var result = await context.Products.FirstOrDefaultAsync(x => x.Id == product.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateProduct_Should_Update_A_Product()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var product = new Product { Name = "Test Product Update", Price = 20m };
        var newProduct = new Product { Name = "Updated Product", Price = 123m };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var updatedProduct = await productService.UpdateProduct(product.Id, newProduct);

        updatedProduct.Should().NotBeNull();
        updatedProduct.Name.Should().Be("Updated Product");
        updatedProduct.Price.Should().Be(123m);
    }

    [Fact]
    public async Task GetProductById_Should_Throw_ProductNotFoundException_When_Product_Does_Not_Exist()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);

        Func<Task> act = async () => await productService.GetProductById(999);

        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    [Fact]
    public async Task UpdateProduct_Should_Throw_ProductNotFoundException_When_Product_Does_Not_Exist()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var newProduct = new Product { Name = "Non-existent Product", Price = 10m };

        Func<Task> act = async () => await productService.UpdateProduct(999, newProduct);

        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    [Fact]
    public async Task DeleteProduct_Should_Throw_ProductNotFoundException_When_Product_Does_Not_Exist()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);

        Func<Task> act = async () => await productService.DeleteProduct(999);

        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    [Fact]
    public async Task GetProducts_Should_Return_Sorted_Products_By_Name_Descending()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var products = new List<Product>
        {
            new Product { Name = "Product C", Price = 30 },
            new Product { Name = "Product A", Price = 10 },
            new Product { Name = "Product B", Price = 20 }
        };
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        var result = await productService.GetProducts(null, "name", "desc", 1, 10);

        result.Items.Should().BeInDescendingOrder(p => p.Name);
    }

    [Fact]
    public async Task GetProducts_Should_Return_Products_Matching_SearchTerm()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);
        var products = new List<Product>
        {
            new Product { Name = "Apple iPhone", Description = "Latest model" },
            new Product { Name = "Samsung Galaxy", Description = "Android phone" },
            new Product { Name = "Apple Watch", Description = "Smartwatch" }
        };
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        var result = await productService.GetProducts("Apple", null, null, 1, 10);

        result.Items.Should().HaveCount(2);
        result.Items.Should().OnlyContain(p => p.Name.Contains("Apple"));
    }

    [Fact]
    public async Task GetAllProducts_Should_Return_Empty_List_When_No_Products_Exist()
    {
        using var context = CreateContext();
        var productService = new ProductService(context);

        var result = await productService.GetAllProducts();

        result.Should().BeEmpty();
    }
}