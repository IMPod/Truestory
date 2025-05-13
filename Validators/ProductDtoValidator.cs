using FluentValidation;
using System.Text.Json;
using Truestory.WebAPI.DTOs;

namespace Truestory.WebAPI.Validators;

/// <summary>
/// Validator for the <see cref="ProductDto"/> class, ensuring that incoming product data meets required validation rules.
/// Validates properties such as Name and Data to ensure correctness and business logic compliance.
/// </summary>
public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDtoValidator"/> class.
    /// Sets up validation rules for:
    /// - Non-empty Name field
    /// - Data dictionary containing a valid 'price' field (must be a numeric type)
    /// - Price value greater than zero
    /// </summary>
    public ProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Data)
            .Must(d => d != null && d.ContainsKey("price") && d["price"] is decimal or int or double)
            .When(x => x.Data != null)
            .WithMessage("Data must contain a valid 'price' field.");

        RuleFor(x => x.Data)
            .Must(d =>
            {
                var msg = JsonSerializer.Serialize(d);
                if (d.TryGetValue("price", out var value) &&
                    double.TryParse(value?.ToString(), out var price))
                {
                    return price > 0;
                }

                return false;
            })
            .When(x => x.Data != null)
            .WithMessage("Price must be greater than zero.");
    }
}