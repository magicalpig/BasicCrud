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


        CreateMap<CompositionPatchRequestDto, Composition>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) =>
            {
                if (srcMember != null && !string.IsNullOrWhiteSpace(srcMember.ToString()))
                {
                    return true;
                }
                return false;
            }));
        
        // nothing is optional when converting from a Put or Post request DTO
        CreateMap<CompositionPutPostRequestDto, Composition>();
            


    }
}
