using BasicCrud.Enums;

namespace BasicCrud.Dtos;

public class CompositionResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string KeySignatureDisplayName { get; set; }
    public required KeySignature KeySignature { get; set; }
    public int NumberOfMovements { get; set; }
    public Format Format { get; set; }
    public required string ComposerName { get; set; }
}