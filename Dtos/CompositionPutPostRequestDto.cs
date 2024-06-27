using System.ComponentModel.DataAnnotations;

namespace BasicCrud.Dtos;

//  Just like the base class except all fields are required
public class CompositionPutPostRequestDto : CompositionRequestDto, IValidatableObject
{
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }
        if (KeySignature == null)
        {
            yield return new ValidationResult("Key signature is required", [nameof(KeySignature)]);
        }

        if (NumberOfMovements == null)
        {
            yield return new ValidationResult("Number of movements is required", [nameof(NumberOfMovements)]);
        }

        if (Format == null)
        {
            yield return new ValidationResult("Format is required", [nameof(Format)]);
        }

        if (string.IsNullOrWhiteSpace(ComposerName) && ComposerId == null)
        {
            yield return new ValidationResult("Composer name or id is required", [nameof(ComposerName), nameof(ComposerId)]);
        }
    }
}