using AutoMapper;
using TipMolde.Application.Dtos.RevisaoDto;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado ao agregado Revisao.
    /// </summary>
    /// <remarks>
    /// Centraliza o mapping entre Dtos da feature Revisao e a entidade de dominio,
    /// evitando construcao manual da entidade no controller.
    /// </remarks>
    public class RevisaoProfile : Profile
    {
        /// <summary>
        /// Configura os mapeamentos da feature Revisao.
        /// </summary>
        public RevisaoProfile()
        {
            CreateMap<CreateRevisaoDto, Revisao>()
                .ForMember(dest => dest.Revisao_id, opt => opt.Ignore())
                .ForMember(dest => dest.NumRevisao, opt => opt.Ignore())
                .ForMember(dest => dest.DataEnvioCliente, opt => opt.Ignore())
                .ForMember(dest => dest.Aprovado, opt => opt.Ignore())
                .ForMember(dest => dest.DataResposta, opt => opt.Ignore())
                .ForMember(dest => dest.FeedbackTexto, opt => opt.Ignore())
                .ForMember(dest => dest.FeedbackImagemPath, opt => opt.Ignore())
                .ForMember(dest => dest.Projeto, opt => opt.Ignore())
                .ForMember(dest => dest.DescricaoAlteracoes, opt => opt.MapFrom(src => src.DescricaoAlteracoes.Trim()));

            CreateMap<Revisao, ResponseRevisaoDto>();
        }
    }
}
