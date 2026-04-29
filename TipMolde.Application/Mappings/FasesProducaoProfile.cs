using AutoMapper;
using TipMolde.Application.Dtos.FasesProducaoDto;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado a feature FasesProducao.
    /// </summary>
    /// <remarks>
    /// Centraliza a conversao entre DTOs e entidade para evitar mapping manual no controller.
    /// </remarks>
    public class FasesProducaoProfile : Profile
    {
        /// <summary>
        /// Configura os mapeamentos da feature.
        /// </summary>
        public FasesProducaoProfile()
        {
            CreateMap<CreateFasesProducaoDto, FasesProducao>()
                .ForMember(dest => dest.Fases_producao_id, opt => opt.Ignore())
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .MapTrimmedOptional(dest => dest.Descricao, src => src.Descricao)
                .ForMember(dest => dest.MaquinasDedicadas, opt => opt.Ignore());

            CreateMap<UpdateFasesProducaoDto, FasesProducao>()
                .MapValueIfProvided(dest => dest.Nome, src => src.Nome)
                .MapTrimmedIfProvided(dest => dest.Descricao, src => src.Descricao)
                .ForMember(dest => dest.Fases_producao_id, opt => opt.Ignore())
                .ForMember(dest => dest.MaquinasDedicadas, opt => opt.Ignore());

            CreateMap<FasesProducao, ResponseFasesProducaoDto>()
                .ForMember(dest => dest.FasesProducao_id, opt => opt.MapFrom(src => src.Fases_producao_id));
        }
    }
}
