using AutoMapper;
using TipMolde.Application.Dtos.RegistoProducaoDto;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado ao agregado RegistosProducao.
    /// </summary>
    /// <remarks>
    /// Centraliza a conversao entre DTOs e entidade para evitar mapping manual
    /// no controller e manter o contrato HTTP separado do modelo de dominio.
    /// </remarks>
    public class RegistosProducaoProfile : Profile
    {
        /// <summary>
        /// Configura os mapeamentos da feature RegistosProducao.
        /// </summary>
        public RegistosProducaoProfile()
        {
            CreateMap<CreateRegistosProducaoDto, RegistosProducao>()
                .ForMember(dest => dest.Registo_Producao_id, opt => opt.Ignore())
                .ForMember(dest => dest.Data_hora, opt => opt.Ignore())
                .ForMember(dest => dest.Estado_producao, opt => opt.MapFrom(src => src.Estado_producao!.Value))
                .ForMember(dest => dest.Fase, opt => opt.Ignore())
                .ForMember(dest => dest.Operador, opt => opt.Ignore())
                .ForMember(dest => dest.Peca, opt => opt.Ignore())
                .ForMember(dest => dest.Maquina, opt => opt.Ignore());

            CreateMap<RegistosProducao, ResponseRegistosProducaoDto>();
        }
    }
}
