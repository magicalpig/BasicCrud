using BasicCrud.Dtos;
using BasicCrud.Persistence;
using BasicCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicCrud.Enums;
using BasicCrud.Enums.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BasicCrud.Services;


namespace BasicCrud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompositionController(CompositionService compositionService, DataContext context, IMapper mapper) : ControllerBase
{
    private readonly DataContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly CompositionService _compositionService = compositionService;

    [HttpGet]
    public async Task<ActionResult<List<CompositionResponseDto>>> GetCompositions([FromQuery] CompositionQueryParameters? queryParameters)
    {
        try 
        {
            var comps = await _compositionService.GetCompositionsAsync(queryParameters);
            if (comps is null) return NotFound();
            return Ok(_mapper.Map<List<Composition>, List<CompositionResponseDto>>(comps));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<ActionResult<CompositionResponseDto>> GetComposition(Guid id)
    {
        var compResponse = await _compositionService.GetCompositionAsync(id);
        if (compResponse is null)
            return NotFound();
        return Ok(_mapper.Map<Composition, CompositionResponseDto>(compResponse));        
    }

    [HttpDelete]
    [Route("{id:Guid}")]
    public async Task<IActionResult> DeleteComposition(Guid id)
    {
        return await _compositionService.DeleteCompositionAsync(id) ? NoContent() : NotFound();
    }

    [HttpPatch]
    [Route("{id:Guid}")]
    public async Task<IActionResult> PatchComposition([FromRoute] Guid id, [FromBody] CompositionPatchRequestDto compositionPatchRequestDto)
    {
        try
        {
            Composition? existingComposition = await _compositionService.GetCompositionAsync(id);
            if (existingComposition is null)
            {
                return NotFound("Composition not found");
            }
            // this will map only the non-null properties from the request DTO to the existing composition
            var mergedComposition = _mapper.Map<CompositionPatchRequestDto, Composition>(compositionPatchRequestDto, existingComposition);

            Composition updatedComposition = await _compositionService.UpdateCompositionAsync(
                mergedComposition,
                compositionPatchRequestDto.ComposerId,
                compositionPatchRequestDto.ComposerName
            );
            return Ok(_mapper.Map<Composition, CompositionResponseDto>(updatedComposition));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
   
    [HttpPost]
    public async Task<ActionResult<CompositionResponseDto>> PostComposition([FromBody] CompositionPutPostRequestDto composition)
    {
        try
        {
            Composition insertedComposition = await _compositionService.CreateCompositionAsync(_mapper.Map<Composition>(composition), composition.ComposerId, composition.ComposerName);
            return Ok(_mapper.Map<Composition, CompositionResponseDto>(insertedComposition));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }        
    }
}