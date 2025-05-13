using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using Truestory.WebAPI.Infrastructure.Settings;

namespace Truestory.WebAPI.Features.Products.Create;

/// <summary>
/// Handler for the <see cref="CreateProductCommand"/> that sends a request to create a product via an external HTTP API.
/// Responsible for mapping command data, sending the HTTP POST request, and returning the raw JSON response.
/// </summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, JsonElement>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CreateProductHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductHandler"/> class.
    /// Configures the <see cref="HttpClient"/> with base address and default headers from application settings.
    /// </summary>
    /// <param name="factory">Factory used to create instances of <see cref="HttpClient"/>.</param>
    /// <param name="apiSettingsAccessor">Provides access to API configuration settings such as the base URL.</param>
    /// <param name="logger">Logger instance for capturing diagnostic and operational information.</param>
    public CreateProductHandler(IHttpClientFactory factory, IOptions<ApiSettings> apiSettingsAccessor, ILogger<CreateProductHandler> logger)
    {
        var apiSettings = apiSettingsAccessor.Value;
        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri(apiSettings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _logger = logger;
    }

    /// <summary>
    /// Handles the incoming <see cref="CreateProductCommand"/>.
    /// Sends a POST request to the configured API endpoint to create a product and returns the raw JSON response.
    /// </summary>
    /// <param name="request">The <see cref="CreateProductCommand"/> containing the product creation data.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="JsonElement"/> representing the API's response.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails or returns a non-success status code.</exception>
    public async Task<JsonElement> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product: {Name}", request.Name);

        var body = new
        {
            name = request.Name,
            data = request.Data
        };

        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/objects", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create product. StatusCode: {StatusCode}", response.StatusCode);
            throw new HttpRequestException("Failed to create product");
        }

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<JsonElement>(result);
    }
}