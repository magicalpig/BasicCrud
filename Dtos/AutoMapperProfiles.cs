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
                o => o.MapFrom(s => DisplayNameHelper.GetDisplayName(s.KeySignature)))
            .ForMember(d => d.ComposerName,
                o => o.MapFrom(s => s.Composer.Name));

        // For the Format and KeySignature enum properties, the DTO can have null while the Composition cannot.
        // This creates a problem where the null becomes a 0 which then gets mapped to the Composition.
        // Using PreCondition to check if the value is null before mapping solves this problem.
        CreateMap<CompositionPatchRequestDto, Composition>()
            .ForMember(dest => dest.Format, opt => opt.PreCondition(src => src.Format.HasValue))
            .ForMember(dest => dest.KeySignature, opt => opt.PreCondition(src => src.KeySignature.HasValue))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrWhiteSpace(srcMember.ToString())))
        ;
        
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
