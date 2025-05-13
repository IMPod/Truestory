using FluentValidation;
using MediatR;

namespace Truestory.WebAPI.PipelineBehaviors;

/// <summary>
/// Represents a pipeline behavior in MediatR for automatic validation of requests using FluentValidation.
/// </summary>
/// <typeparam name="TRequest">The type of the request. Must implement <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response associated with the request.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
/// </remarks>
/// <param name="validators">A collection of validators associated with the request type.</param>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request by applying all registered validators.
    /// Throws a <see cref="ValidationException"/> if validation fails.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">Delegate to call the next handler in the pipeline.</param>
    /// <param name="ct">Cancellation token to cancel the operation.</param>
    /// <returns>The result of the request as <typeparamref name="TResponse"/>.</returns>
    /// <exception cref="ValidationException">Thrown when validation of the request fails.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var failures = validators
            .Select(v => v.Validate(new ValidationContext<TRequest>(request)))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}