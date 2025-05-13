using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Truestory.WebAPI.DTOs;
using Truestory.WebAPI.Infrastructure.Settings;
using Truestory.WebAPI.Validators;

namespace Truestory.WebAPI.Features.Products.GetAll;

/// <summary>
/// Handler for the <see cref="GetAllProductsQuery"/> that retrieves a paginated and filtered list of products from an external API.
/// Performs HTTP communication, response validation, filtering, and pagination.
/// </summary>
public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ExternalProductResponse>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GetAllProductsHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllProductsHandler"/> class.
    /// Configures the <see cref="HttpClient"/> with base address and default headers from application settings.
    /// </summary>
    /// <param name="factory">Factory used to create instances of <see cref="HttpClient"/>.</param>
    /// <param name="apiSettingsAccessor">Provides access to API configuration settings such as the base URL.</param>
    /// <param name="logger">Logger instance for capturing diagnostic and operational information.</param>
    public GetAllProductsHandler(IHttpClientFactory factory, IOptions<ApiSettings> apiSettingsAccessor, ILogger<GetAllProductsHandler> logger)
    {
        var apiSettings = apiSettingsAccessor.Value;
        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri(apiSettings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _logger = logger;
    }

    /// <summary>
    /// Handles the incoming <see cref="GetAllProductsQuery"/>.
    /// Retrieves all products from an external API, validates them, applies filtering and pagination, and returns the result.
    /// </summary>
    /// <param name="request">The query containing filter and pagination parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>An enumerable collection of <see cref="ExternalProductResponse"/> objects after filtering and pagination.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails or returns a non-success status code.</exception>
    public async Task<IEnumerable<ExternalProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting products. Filter: {Name}, Page: {Page}, PageSize: {PageSize}",
            request.Name, request.Page, request.PageSize);

        var response = await SendGetRequestAsync(cancellationToken);
        var products = await DeserializeResponseAsync(response, cancellationToken);

        if (!products.Any())
        {
            return [];
        }

        var validProducts = ValidateProducts(products);
        var filteredProducts = ApplyFilteringAndPagination(validProducts, request);

        return filteredProducts;
    }

    private async Task<HttpResponseMessage> SendGetRequestAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("/objects", cancellationToken);

        if (response.IsSuccessStatusCode) 
            return response;
        _logger.LogError("Failed to retrieve products. StatusCode: {StatusCode}", response.StatusCode);
        throw new HttpRequestException("Failed to get data from external API");

    }

    private async Task<List<ExternalProductResponse>> DeserializeResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var products = await response.Content.ReadFromJsonAsync<List<ExternalProductResponse>>(cancellationToken: cancellationToken);

        if (products == null || !products.Any())
        {
            _logger.LogWarning("No products returned from the external API.");
        }

        return products ?? new List<ExternalProductResponse>();
    }

    private IEnumerable<ExternalProductResponse> ValidateProducts(IEnumerable<ExternalProductResponse> products)
    {
        var validator = new ExternalProductResponseValidator();

        return products
            .Where(p =>
            {
                var result = validator.Validate(p);
                if (!result.IsValid)
                {
                    _logger.LogWarning("Skipping invalid product: {Errors}", string.Join(", ", result.Errors));
                }
                return result.IsValid;
            })
            .ToList();
    }

    private IEnumerable<ExternalProductResponse> ApplyFilteringAndPagination(
        IEnumerable<ExternalProductResponse> products,
        GetAllProductsQuery request)
    {
        return products
            .Where(p => string.IsNullOrEmpty(request.Name) ||
                        p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase))
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
    }
}