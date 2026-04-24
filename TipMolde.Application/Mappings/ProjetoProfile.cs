using AutoMapper;
using TipMolde.Application.Dtos.ProjetoDto;
using TipMolde.Application.Dtos.RevisaoDto;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado ao agregado Projeto.
    /// </summary>
    /// <remarks>
    /// Centraliza o mapping entre Dtos e entidades para evitar logica dispersa no controller.
    /// </remarks>
    public class ProjetoProfile : Profile
    {
        /// <summary>
        /// Configura os mapeamentos da feature Projeto.
        /// </summary>
        public ProjetoProfile()
        {
            CreateMap<CreateProjetoDto, Projeto>()
                .ForMember(dest => dest.Projeto_id, opt => opt.Ignore())
                .ForMember(dest => dest.NomeProjeto, opt => opt.MapFrom(src => src.NomeProjeto.Trim()))
                .ForMember(dest => dest.SoftwareUtilizado, opt => opt.MapFrom(src => src.SoftwareUtilizado.Trim()))
                .ForMember(dest => dest.CaminhoPastaServidor, opt => opt.MapFrom(src => src.CaminhoPastaServidor.Trim()))
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.Revisoes, opt => opt.Ignore())
                .ForMember(dest => dest.RegistosTempo, opt => opt.Ignore());

            CreateMap<UpdateProjetoDto, Projeto>()
                .ForMember(dest => dest.NomeProjeto, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.NomeProjeto));
                    opt.MapFrom(src => src.NomeProjeto!.Trim());
                })
                .ForMember(dest => dest.SoftwareUtilizado, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.SoftwareUtilizado));
                    opt.MapFrom(src => src.SoftwareUtilizado!.Trim());
                })
                .ForMember(dest => dest.CaminhoPastaServidor, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.CaminhoPastaServidor));
                    opt.MapFrom(src => src.CaminhoPastaServidor!.Trim());
                })
                .ForMember(dest => dest.TipoProjeto, opt =>
                {
                    opt.Condition(src => src.TipoProjeto.HasValue);
                    opt.MapFrom(src => src.TipoProjeto!.Value);
                })
                .ForMember(dest => dest.Projeto_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.Revisoes, opt => opt.Ignore())
                .ForMember(dest => dest.RegistosTempo, opt => opt.Ignore());

            CreateMap<Projeto, ResponseProjetoDto>();

            CreateMap<Projeto, ResponseProjetoWithRevisoesDto>()
                .ForMember(
                    dest => dest.Revisoes,
                    opt => opt.MapFrom(src => src.Revisoes.OrderByDescending(r => r.NumRevisao)));
        }
    }
}
