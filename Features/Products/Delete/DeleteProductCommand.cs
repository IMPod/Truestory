using MediatR;

namespace Truestory.WebAPI.Features.Products.Delete;

public record DeleteProductCommand(string Id) : IRequest<Unit>;