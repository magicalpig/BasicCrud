using System.ComponentModel.DataAnnotations;
using BasicCrud.Enums;
using BasicCrud.Enums.Helpers;

namespace BasicCrud.Dtos;

// Any validation rules defined in this class will apply to all request method types (PUT, POST, PATCH)
// Subclasses designed for specific request method types can define their own additional validation rules

public abstract class CompositionRequestDto : IValidatableObject
{
    // parameterless constructor needed for JSON deserialization of request payload
    public CompositionRequestDto()
    {
        
    }

    public string? Name { get; set; }

    [EnumDataType(typeof(KeySignature), ErrorMessage = "Invalid key signature")]
    public KeySignature? KeySignature { get; set; }


    private string? _keySignatureDisplayName;
    public string? KeySignatureDisplayName
    {
        get => _keySignatureDisplayName;
        set
        {
            _keySignatureDisplayName = value;
            // Automatically update KeySignature when KeySignatureDisplayName is set
            if (!string.IsNullOrWhiteSpace(_keySignatureDisplayName))
            {
                int? keyCode = DisplayNameHelper.GetEnumValueFromDisplayName<KeySignature>(_keySignatureDisplayName);
                if (keyCode.HasValue)
                {
                    KeySignature = (KeySignature)keyCode;
                }
            }
        }
    }

    [Range(1, int.MaxValue, ErrorMessage = "Not enough or too many movements")]
    public int? NumberOfMovements { get; set; }

    [EnumDataType(typeof(Format), ErrorMessage = "Invalid Format")]
    public Format? Format { get; set; }


    public Guid? ComposerId { get; set; }
    public string? ComposerName { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // could put common validation logic here
        yield break;
    }
}
