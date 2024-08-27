using BasicCrud.Models;
using Microsoft.EntityFrameworkCore;

namespace BasicCrud.Persistence;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Composition> Compositions { get; set; }
    public DbSet<Composer> Composers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Composition>()
            .HasOne(c => c.Composer)
            .WithMany(c => c.Compositions)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public async Task<T?> FindByNameAsync<T>(string name) where T : class, INamedEntity
    {
        return await Set<T>().FirstOrDefaultAsync(x => x.Name == name);
    }
}