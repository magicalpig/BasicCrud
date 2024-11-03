using BasicCrud.Enums;

namespace BasicCrud.Dtos;

public record CompositionQueryParameters(
    string? Name,
    KeySignature? KeySignature,
    string? KeySignatureDisplayName,
    Format? Format,
    string? ComposerName
);