using System.Text.Json.Serialization;
using BasicCrud.Enums;

namespace BasicCrud.Dtos;

public class CompositionResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string KeySignatureDisplayName { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required KeySignature KeySignature { get; set; }
    public int NumberOfMovements { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Format Format { get; set; }
    public required string ComposerName { get; set; }
}