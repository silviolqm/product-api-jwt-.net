using System;

namespace Product_API_JWT.Exceptions;

public class ProductNotFoundException : Exception
{
	public ProductNotFoundException() : base("Product not found.") { }

	public ProductNotFoundException(string message) : base(message) { }

	public ProductNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}