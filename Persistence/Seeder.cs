using BasicCrud.Enums;
using BasicCrud.Models;
using Microsoft.EntityFrameworkCore;


namespace BasicCrud.Persistence;
public class Seeder
{
    public static async Task SeedData(DataContext context, IConfiguration config)
    {
        if (config.GetValue<bool>("Settings:ResetDatabaseOnStartup"))
        {
            context.Compositions.RemoveRange(context.Compositions);
            context.Composers.RemoveRange(context.Composers);
            context.SaveChanges();
        }

        if (!context.Composers.Any())
        {
            var composers = new List<Composer>
            {
                new () { Name = "Beethoven" },
                new () { Name = "Bach" },
                new () { Name = "Mozart" },
                new () { Name = "Chopin" },
                new () { Name = "Sibelius" },
                new () { Name = "Shostaovich" },
                new () { Name = "Brahms" }
            };

            await context.Composers.AddRangeAsync(composers);
            await context.SaveChangesAsync();
        }

        if (!context.Compositions.Any())
        {
            var composers = await context.Composers.ToListAsync();
            Dictionary<string, Composer> composerDictionary = composers.ToDictionary(c => c.Name);
            var compositions = new List<Composition>
            {
                new ()
                {
                    Name = "Symphony No. 5",
                    KeySignature = KeySignature.CMinor,
                    Format = Format.Symphony,
                    Composer = composerDictionary["Beethoven"],
                    NumberOfMovements = 4
                },
                new ()
                {
                    Name = "Brandenburg Concerto No. 3",
                    KeySignature = KeySignature.GMajor,
                    Format = Format.Concerto,
                    Composer = composerDictionary["Bach"],
                    NumberOfMovements = 3
                },
                new ()
                {
                    Name = "Violin Concerto",
                    KeySignature = KeySignature.DMinor,
                    Format = Format.Concerto,
                    Composer = composerDictionary["Sibelius"],
                    NumberOfMovements = 3
                },
                new ()
                {
                    Name = "Violin Concerto",
                    KeySignature = KeySignature.DMajor,
                    Format = Format.Concerto,
                    Composer = composerDictionary["Brahms"],
                    NumberOfMovements = 3
                },
                new ()
                {
                    Name = "Piano Sonata No. 2",
                    KeySignature = KeySignature.BFlatMinor,
                    Format = Format.Sonata,
                    Composer = composerDictionary["Chopin"],
                    NumberOfMovements = 4
                }
            };

            await context.Compositions.AddRangeAsync(compositions);
            await context.SaveChangesAsync();
        }
    }
}
