using AutoMapper;
using BasicCrud.Models;
using BasicCrud.Enums.Helpers;

namespace BasicCrud.Dtos;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Composition, CompositionResponseDto>()
            .ForMember(d => d.KeySignatureDisplayName,
                o => o.MapFrom(s => s.KeySignature != null 
                    ?   DisplayNameHelper.GetDisplayName(s.KeySignature)
                    : "Unknown"))
            .ForMember(d => d.ComposerName,
                o => o.MapFrom(s => s.Composer.Name));

        CreateMap<CompositionPatchRequestDto, Composition>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
            {
                // because Composition's NumberOfMovements is non-nullable, a null on the 
                // source side (the DTO) will be converted to 0 by automapper.
                // Prevent that 0 from being copied to destination Composition
                if (opts.DestinationMember.Name == nameof(Composition.NumberOfMovements))
                {
                    int? numberOfMovements = (src as CompositionPatchRequestDto).NumberOfMovements;
                    // Check if the source value has a value and is greater than 0
                    return numberOfMovements.HasValue == true && numberOfMovements > 0;
                }
                return srcMember != null && !string.IsNullOrWhiteSpace(srcMember?.ToString());
            }));



        // nothing is optional when converting from a Put or Post request DTO
        CreateMap<CompositionPutPostRequestDto, Composition>();

        // Map from Composition to Composition.
        // Preserve ID of the destination object.
        // Only map properties that are not null.
        CreateMap<Composition, Composition>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
