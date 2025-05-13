using MediatR;
using Truestory.WebAPI.DTOs;

namespace Truestory.WebAPI.Features.Products.GetAll;

public record GetAllProductsQuery(string? Name, int Page, int PageSize) : IRequest<IEnumerable<ExternalProductResponse>>;