using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using Truestory.WebAPI.DTOs;
using Truestory.WebAPI.Features.Products.Create;
using Truestory.WebAPI.Features.Products.Delete;
using Truestory.WebAPI.Features.Products.GetAll;

namespace Truestory.WebAPI;

/// <summary>
/// Controller for interacting with the mock product API (https://restful-api.dev).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Retrieves products from the mock API with optional filtering and paging.
    /// </summary>
    /// <param name="name">Optional filter by name substring.</param>
    /// <param name="page">Page number for pagination (default = 1).</param>
    /// <param name="pageSize">Number of items per page (default = 10).</param>
    /// <returns>Filtered and paginated list of products.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get products",
        Description = "Retrieves products from the mock API with optional filtering by name and pagination."
    )]
    [SwaggerResponse(200, "Returns a list of products", typeof(IEnumerable<JsonElement>))]
    [SwaggerResponse(500, "Failed to get data from external API")]
    public async Task<IActionResult> GetAll(string? name = null, int page = 1, int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllProductsQuery(name, page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new product in the mock API.
    /// </summary>
    /// <param name="dto">Product data including name and custom fields.</param>
    /// <returns>The created product object.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create product",
        Description = "Creates a new product using the external mock API."
    )]
    [SwaggerResponse(200, "Product created successfully", typeof(ProductDto))]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(500, "Failed to create product")]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
    {
        var result = await mediator.Send(new CreateProductCommand(dto.Name, dto.Data));
        return Ok(result);
    }

    /// <summary>
    /// Deletes a product by ID from the mock API.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete product",
        Description = "Deletes a product by its ID using the external mock API."
    )]
    [SwaggerResponse(204, "Product deleted successfully")]
    [SwaggerResponse(404, "Product not found")]
    [SwaggerResponse(500, "Failed to delete product")]
    public async Task<IActionResult> Delete(string id)
    {
        await mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}