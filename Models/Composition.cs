using BasicCrud.Enums;
using BasicCrud.Persistence;
namespace BasicCrud.Models;


public class Composition : INamedEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required KeySignature KeySignature { get; set; }
    public int NumberOfMovements { get; set; } = 1;
    public Format Format { get; set; } = Format.Other;
    public required Composer Composer { get; set; }
}