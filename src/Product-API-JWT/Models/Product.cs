using System;
using System.ComponentModel.DataAnnotations;

namespace Product_API_JWT.Model;

public class Product
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Price { get; set; }
}
