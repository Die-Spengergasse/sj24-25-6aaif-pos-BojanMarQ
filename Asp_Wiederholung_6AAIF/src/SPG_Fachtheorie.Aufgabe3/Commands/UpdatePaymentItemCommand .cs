using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands
{
    public record UpdatePaymentItemCommand(
        [Range(1, 999999, ErrorMessage = "Invalid payment item id")]
        int Id,
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid article name")]
        string ArticleName,
        [Range(1, 999999, ErrorMessage = "Invalid amount")]
        int Amount,
        [Range(0.01, 999999.99, ErrorMessage = "Invalid price")]
        decimal Price,
        DateTime? LastUpdated
    ) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ArticleName))
                yield return new ValidationResult("Article name is required", new[] { nameof(ArticleName) });
            if (Amount <= 0)
                yield return new ValidationResult("Amount must be greater than zero", new[] { nameof(Amount) });
            if (Price <= 0)
                yield return new ValidationResult("Price must be greater than zero", new[] { nameof(Price) });
        }
    }
}