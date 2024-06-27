using BasicCrud.Dtos;
using BasicCrud.Persistence;
using BasicCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicCrud.Enums;
using BasicCrud.Enums.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;


namespace BasicCrud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompositionController(DataContext context, IMapper mapper) : ControllerBase
{
    private readonly DataContext _context = context;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<List<CompositionResponseDto>>> GetCompositions([FromQuery] CompositionQueryParameters? queryParameters)
    {
        var query = _context.Compositions
            .Include(c => c.Composer)
            .AsQueryable();

        if (queryParameters is not null)
        {
            // Name of composition
            if (!string.IsNullOrWhiteSpace(queryParameters.Name))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{queryParameters.Name}%"));
            }

            // Key Signature as integer
            if (queryParameters.KeySignature is not null)
            {
                query = query.Where(c => c.KeySignature == queryParameters.KeySignature);
            }

            // Key Signature as display name string
            if (!string.IsNullOrWhiteSpace(queryParameters.KeySignatureDisplayName))
            {
                int? keyCode = DisplayNameHelper
                    .GetEnumValueFromDisplayName<KeySignature>(queryParameters.KeySignatureDisplayName);

                if (keyCode is null)
                {
                    return BadRequest("Unknown value provided for KeySignatureDisplayName filter.");
                }
                else
                {
                    query = query.Where(c => c.KeySignature == (KeySignature)keyCode);
                }
            }

            // Format as integer
            if (queryParameters.Format is not null)
            {
                query = query.Where(c => c.Format == queryParameters.Format);
            }
            //TODO provide a way to allow the text of a Format (e.g. "Sonata") to be used as a filter

            // Composer name
            if (!string.IsNullOrWhiteSpace(queryParameters.ComposerName))
            {
                query = query.Where(c => EF.Functions.Like(c.Composer.Name, $"%{queryParameters.ComposerName}%"));
            }
        }

        var comps = await query
            .ProjectTo<CompositionResponseDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (comps is null)
        {
            return NotFound();
        }
        return Ok(comps);
    }

    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<ActionResult<Composition>> GetComposition(Guid id)
    {
        var compResponse = await _context.Compositions
            .Where(c => c.Id == id)
            .ProjectTo<CompositionResponseDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        if (compResponse is null)
            return NotFound();
        return Ok(compResponse);
    }

    [HttpDelete]
    [Route("{id:Guid}")]
    public async Task<IActionResult> DeleteComposition(Guid id)
    {
        var comp = await _context.Compositions.FindAsync(id);
        if (comp is null)
            return NotFound();
        _context.Compositions.Remove(comp);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch]
    [Route("{id:Guid}")]
    public async Task<IActionResult> PatchComposition([FromRoute] Guid id, [FromBody] CompositionPatchRequestDto composition)
    {
        var existing = await _context.Compositions.FindAsync(id);
        if (existing is null)
            return NotFound();
        await _context.Entry(existing).Reference(x => x.Composer).LoadAsync();

        Composer composerReference;
        if (composition.ComposerId is not null)
        {
            var composer = await _context.Composers.FindAsync(composition.ComposerId);
            if (composer is null)
                return BadRequest("Composer not found for the provided id");
            composerReference = composer;
        }
        // Composer was passed by name instead of by guid
        else if (!string.IsNullOrWhiteSpace(composition.ComposerName))
        {
            var existingComposer = await _context.Composers.FirstOrDefaultAsync(c => c.Name == composition.ComposerName);
            if (existingComposer is not null)
                composerReference = existingComposer;
            else
            {
                var newComposer = new Composer { Name = composition.ComposerName };
                _context.Composers.Add(newComposer);
                
                composerReference = newComposer;
            }
        }
        else
        {
            composerReference = existing.Composer;
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // the composer is new. it needs to be committed.
            if (_context.Entry<Composer>(composerReference).State == EntityState.Added)
            {
                await _context.SaveChangesAsync();
            }            
            
            var modified = _mapper.Map<CompositionPatchRequestDto, Composition>(composition, existing);
            modified.Composer = composerReference;

            Composition toReturn;
            if (_context.Entry(modified).State == EntityState.Modified)
            {
                var successfulUpdate = await _context.SaveChangesAsync() > 0;

                if (successfulUpdate)
                {
                    await transaction.CommitAsync();
                    toReturn = modified;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Failed to update composition");
                }                        
            }
            else // return existing as-is
            {
                toReturn = existing;
            }
            
             
            return Ok(_mapper.Map<Composition, CompositionResponseDto>(toReturn));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CompositionResponseDto>> PostComposition([FromBody] CompositionPutPostRequestDto composition)
    {
        Composer composerReference;
        if (composition.ComposerId is not null)
        {
            var composer = await _context.Composers.FindAsync(composition.ComposerId);
            if (composer is null)
                return BadRequest("Composer not found for the provided id");
            composerReference = composer;
        }
        // Composer was passed by name instead of by guid
        else if (!string.IsNullOrWhiteSpace(composition.ComposerName))
        {
            var existingComposer = await _context.Composers.FirstOrDefaultAsync(c => c.Name == composition.ComposerName);
            if (existingComposer is not null)
                composerReference = existingComposer;
            else
            {
                var newComposer = new Composer { Name = composition.ComposerName };
                _context.Composers.Add(newComposer);
                
                composerReference = newComposer;
            }
        }
        else // this case should have been caught by validators directly on the DTO properties
        {
            return BadRequest("Composer name or id is required");
        }
        
        var existingRecord = await _context.Compositions
            .AnyAsync(c => c.Name == composition.Name 
                        && c.KeySignature == composition.KeySignature 
                        && c.Composer.Id == composerReference.Id);
        if (existingRecord)
        {
            return Conflict("A Composition with the same Name, KeySignature, and Composer already exists.");
        }


        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // the composer is new. it needs to be committed.
            if (_context.Entry<Composer>(composerReference).State == EntityState.Added)
            {
                await _context.SaveChangesAsync();
            }            
            
            Composition newRecord = _mapper.Map<CompositionPutPostRequestDto, Composition>(composition);
            newRecord.Composer = composerReference;
            _context.Compositions.Add(newRecord);
                            
            var successfulInsert = await _context.SaveChangesAsync() > 0;
            if (successfulInsert)
            {
                await transaction.CommitAsync();
                
                var response = _mapper.Map<Composition, CompositionResponseDto>(newRecord);
                return Ok(response);                  
            }
            else
            {
                await transaction.RollbackAsync();
                return BadRequest("Failed to insert composition");
            }                        

        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return BadRequest(e.Message);
        }
    }
}