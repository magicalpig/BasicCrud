using BasicCrud.Enums;
using BasicCrud.Persistence;
namespace BasicCrud.Models;


public class Composition : INamedEntity
{
    // some properties are nullable but the service layer will prevent
    // the object from being saved if they are null. I did this so automapper
    // would not convert nulls in DTO to 0s when going from DTO to Composition.
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public KeySignature? KeySignature { get; set; }
    public int NumberOfMovements { get; set; } = 1;
    public Format? Format { get; set; }
    public required Composer Composer { get; set; }
}