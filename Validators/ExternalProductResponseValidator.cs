using FluentValidation;
using Truestory.WebAPI.DTOs;

namespace Truestory.WebAPI.Validators;

/// <summary>
/// Validator for the <see cref="ExternalProductResponse"/> class, ensuring that product response data from external sources meets required validation rules.
/// Validates core properties such as Id and Name to ensure data integrity and contract compliance.
/// </summary>
public class ExternalProductResponseValidator : AbstractValidator<ExternalProductResponse>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalProductResponseValidator"/> class.
    /// Sets up validation rules for:
    /// - Non-empty Id field
    /// - Non-empty Name field
    /// 
    /// Currently commented rules are reserved for future validation of the Data dictionary (e.g., price validation).
    /// </summary>
    public ExternalProductResponseValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        //RuleFor(x => x.Data).NotNull();
        //RuleFor(x => x.Data)
        //    .Must(d => d.ContainsKey("price") && d["price"] is decimal or int)
        //    .WithMessage("Response data must contain a numeric 'price' field.");
    }
}