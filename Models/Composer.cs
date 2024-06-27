using BasicCrud.Persistence;

namespace BasicCrud.Models;

public class Composer : INamedEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Composition> Compositions { get; set; } = [];
}