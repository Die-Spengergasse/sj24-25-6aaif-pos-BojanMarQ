using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands
{
    public class UpdateConfirmedCommand : IValidatableObject
    {
        public DateTime? Confirmed { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Confirmed.HasValue)
            {
                var now = DateTime.UtcNow;
                if (Confirmed.Value > now.AddMinutes(1))
                {
                    yield return new ValidationResult(
                        "The confirmed date cannot be more than 1 minute in the future.",
                        new[] { nameof(Confirmed) }
                    );
                }
            }
        }
    }
}