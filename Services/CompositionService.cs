using AutoMapper;
using BasicCrud.Dtos;
using BasicCrud.Enums;
using BasicCrud.Enums.Helpers;
using BasicCrud.Models;
using BasicCrud.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BasicCrud.Services;


public class CompositionService(DataContext context, IMapper mapper)
{
    private readonly DataContext _context = context;
    private readonly IMapper _mapper = mapper;

    private static void ValidateComposition(Composition composition)
    {
        if (composition.KeySignature == null)
        {
            throw new ArgumentException("KeySignature is required.");
        }
        else if (composition.NumberOfMovements < 1)
        {
            throw new ArgumentException("NumberOfMovements must be at least 1.");
        }
        else if (composition.Format == null)
        {
            throw new ArgumentException("Format is required.");
        }
    }

    public async Task<bool> DeleteCompositionAsync(Guid id)
    {
        var composition = await _context.Compositions.FindAsync(id);
        if (composition is null) return false;
        _context.Remove(composition);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Composition?> GetCompositionAsync(Guid id)
    {
        Composition? compResponse = await _context.Compositions
            .Include(c => c.Composer)
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
        return compResponse;
    }

    public async Task<List<Composition>> GetCompositionsAsync(CompositionQueryParameters? queryParameters)
    {
        IQueryable<Composition> query = _context.Compositions
            .Include(c => c.Composer); // explicit typing otherwise we have IIncludableQueryable

        if (queryParameters is not null)
        {
            // filter includes Name of composition
            if (!string.IsNullOrWhiteSpace(queryParameters.Name))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{queryParameters.Name}%"));
            }

            // filter includes Key Signature as integer
            if (queryParameters.KeySignature is not null)
            {
                query = query.Where(c => c.KeySignature == queryParameters.KeySignature);
            }

            // filter includes Key Signature as display name string
            if (!string.IsNullOrWhiteSpace(queryParameters.KeySignatureDisplayName))
            {
                int? keyCode = DisplayNameHelper
                    .GetEnumValueFromDisplayName<KeySignature>(queryParameters.KeySignatureDisplayName);

                if (keyCode is null)
                {
                    throw new ArgumentException("Unknown value provided for KeySignatureDisplayName filter.");
                }
                query = query.Where(c => c.KeySignature == (KeySignature)keyCode);
            }

            // filter includes Format as integer
            if (queryParameters.Format is not null)
            {
                query = query.Where(c => c.Format == queryParameters.Format);
            }

            // filter includes Composer name
            if (!string.IsNullOrWhiteSpace(queryParameters.ComposerName))
            {
                query = query.Where(c => EF.Functions.Like(c.Composer.Name, $"%{queryParameters.ComposerName}%"));
            }
        }

        var comps = await query.ToListAsync();
        return comps;
    }


    public async Task<Composition> CreateCompositionAsync(Composition composition, Guid? composerId, string? composerName)
    {
        Composer composerReference;
        if (composerId is not null)
        {
            var composer = await _context.Composers.FindAsync(composerId) ?? throw new ArgumentException("Composer not found for the provided id");
            composerReference = composer;
        }
        // Composer was passed by name instead of by guid
        else if (!string.IsNullOrWhiteSpace(composerName))
        {
            var existingComposer = await _context.Composers.FirstOrDefaultAsync(c => c.Name == composerName);
            if (existingComposer is not null)
                composerReference = existingComposer;
            else
            {
                var newComposer = new Composer { Name = composerName };
                _context.Composers.Add(newComposer);

                composerReference = newComposer;
            }
        }
        else // this case should have been caught by validators directly on the DTO properties
        {
            throw new ArgumentException("Composer name or id is required");
        }

        var existingRecord = await _context.Compositions
            .AnyAsync(c => c.Name == composition.Name
                        && c.KeySignature == composition.KeySignature
                        && c.Composer.Id == composerReference.Id);
        if (existingRecord)
        {
            throw new InvalidOperationException("A Composition with the same Name, KeySignature, and Composer already exists.");
        }

        ValidateComposition(composition);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // the composer is new. it needs to be committed.
            if (_context.Entry<Composer>(composerReference).State == EntityState.Added)
            {
                await _context.SaveChangesAsync();
            }

            composition.Composer = composerReference;
            _context.Compositions.Add(composition);

            var successfulInsert = await _context.SaveChangesAsync() > 0;
            if (successfulInsert)
            {
                await transaction.CommitAsync();
                return composition;
            }
            else
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Failed to insert composition");
            }

        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task<Composition> UpdateCompositionAsync(Composition composition, Guid? composerId, string? composerName)
    {
        var existingId = composition.Id;
        var existingComposition = await _context.Compositions.FindAsync(existingId) ?? throw new KeyNotFoundException("Composition not found for the provided id");

        await _context.Entry(existingComposition).Reference(x => x.Composer).LoadAsync();

        // if caller wants to change composer of the composition, it will pass in a composer guid or composer name
        Composer? composerReference = existingComposition.Composer;
        // composer was passed by guid
        if (composerId is not null)
        {
            composerReference = await _context.Composers.FindAsync(composerId) ?? throw new KeyNotFoundException("Composer not found for the provided id");
        }
        // Composer was passed by name instead of by guid
        else if (!string.IsNullOrWhiteSpace(composerName))
        {
            // check whether the composer name has been used before for any composition
            composerReference = await _context.Composers.FirstOrDefaultAsync(c => c.Name == composerName);
            // this composer name has not been used before. Add composer to the database.
            if (composerReference is null)
            {
                composerReference = _context.Composers.Add(new Composer { Name = composerName }).Entity;
            }
        }

        ValidateComposition(composition);
        
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // the composer is new. it needs to be committed.
            if (_context.Entry<Composer>(composerReference).State == EntityState.Added)
            {
                await _context.SaveChangesAsync();
            }

            // only the "existing" object is being tracked by the context, so that's the one to use in db operations.
            // copy any changes from the incoming object to the tracked object.
            _mapper.Map(composition, existingComposition);
            existingComposition.Composer = composerReference;

            if (_context.Entry(existingComposition).State == EntityState.Modified)
            {
                var successfulUpdate = await _context.SaveChangesAsync() > 0;

                if (successfulUpdate)
                {
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("Failed to update composition or insert new composer");
                }
            }
            return existingComposition;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException(e.Message);
        }
    }
}