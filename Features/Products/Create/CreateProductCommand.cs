using MediatR;
using System.Text.Json;

namespace Truestory.WebAPI.Features.Products.Create;

/// <summary>
/// Represents a command to create a new product in the system.
/// Contains the necessary data required to perform the product creation operation via the API.
/// </summary>
/// <param name="Name">The name of the product. This field is required.</param>
/// <param name="Data">Optional additional metadata or properties related to the product, such as pricing or custom fields.</param>
public record CreateProductCommand(string Name, object? Data) : IRequest<JsonElement>;