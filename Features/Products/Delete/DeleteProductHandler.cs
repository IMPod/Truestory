using MediatR;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Options;
using Truestory.WebAPI.Infrastructure.Settings;

namespace Truestory.WebAPI.Features.Products.Delete;

/// <summary>
/// Handler for the <see cref="DeleteProductCommand"/> that sends a request to delete a product from an external API.
/// Ensure proper logging, error handling, and communication with the remote service.
/// </summary>
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeleteProductHandler> _logger;
    private readonly ApiSettings _apiSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProductHandler"/> class.
    /// Configures the <see cref="HttpClient"/> with base address and default headers from application settings.
    /// </summary>
    /// <param name="factory">Factory used to create instances of <see cref="HttpClient"/>.</param>
    /// <param name="apiSettingsAccessor">Provides access to API configuration settings such as the base URL.</param>
    /// <param name="logger">Logger instance for capturing diagnostic and operational information.</param>
    public DeleteProductHandler(IHttpClientFactory factory, IOptions<ApiSettings> apiSettingsAccessor, ILogger<DeleteProductHandler> logger)
    {
        _apiSettings = apiSettingsAccessor.Value;
        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl); 
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _logger = logger;
    }

    /// <summary>
    /// Handles the incoming <see cref="DeleteProductCommand"/>.
    /// Sends a DELETE request to remove a product by ID and returns a <see cref="Unit"/> result upon success.
    /// </summary>
    /// <param name="request">The command containing the ID of the product to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="Unit"/> value indicating successful execution.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the requested product does not exist (HTTP 404).</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails or returns a non-success status code.</exception>
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product with ID: {Id}", request.Id);

        var response = await _httpClient.DeleteAsync($"/objects/{request.Id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Product with ID {Id} not found", request.Id);
            throw new KeyNotFoundException("Product not found");
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to delete product. StatusCode: {StatusCode}", response.StatusCode);
            throw new HttpRequestException("Failed to delete product");
        }

        _logger.LogInformation("Product with ID {Id} deleted successfully", request.Id);
        return Unit.Value;
    }
}