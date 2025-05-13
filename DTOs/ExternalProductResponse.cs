namespace Truestory.WebAPI.DTOs;

/// <summary>
/// Represents a product response from an external API.
/// Contains core product data such as ID, name, and additional metadata.
/// </summary>
public class ExternalProductResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets a dictionary of additional metadata associated with the product,
    /// such as price, description, or custom fields.
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = null!;
}