using System.ComponentModel.DataAnnotations;
using BasicCrud.Enums;

namespace BasicCrud.Dtos;

//  Just like the base class except all fields are required
public class CompositionPutPostRequestDto : CompositionRequestDto, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
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
            yield return new ValidationResult("Format is required",  [nameof(Format)]);
        }

        if (string.IsNullOrWhiteSpace(ComposerName) && ComposerId == null)
        {
            yield return new ValidationResult("Composer name or id is required",[nameof(ComposerName), nameof(ComposerId)]);
        }
    }
}
