namespace Truestory.WebAPI.DTOs;

/// <summary>
/// Data Transfer Object (DTO) representing a product in the system.
/// Used to transfer product data between processes, such as receiving input from an API request.
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Gets or sets the name of the product. This field is required.
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Gets or sets additional metadata or custom properties of the product, such as price, tags, or other dynamic fields.
    /// This is typically stored as key-value pairs.
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }
}