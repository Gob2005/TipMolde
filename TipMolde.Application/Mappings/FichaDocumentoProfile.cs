using AutoMapper;
using TipMolde.Application.Dtos.FichaDocumentoDto;
using TipMolde.Domain.Entities.Fichas;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado ao agregado documental das fichas de producao.
    /// </summary>
    public class FichaDocumentoProfile : Profile
    {
        public FichaDocumentoProfile()
        {
            CreateMap<CreateFichaDocumentoDto, FichaDocumento>()
                .ForMember(dest => dest.FichaDocumento_id, opt => opt.Ignore())
                .ForMember(dest => dest.FichaProducao, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoPor, opt => opt.Ignore());

            CreateMap<FichaDocumento, ResponseFichaDocumentoDto>();
        }
    }
}
