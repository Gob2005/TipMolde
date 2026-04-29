using AutoMapper;
using TipMolde.Application.Dtos.ProjetoDto;
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
            ConfigureCreateMap();
            ConfigureUpdateMap();
            ConfigureResponseMap();
            ConfigureResponseWithRevisoesMap();
        }

        private void ConfigureCreateMap()
        {
            CreateMap<CreateProjetoDto, Projeto>()
                .ForMember(dest => dest.Projeto_id, opt => opt.Ignore())
                .MapTrimmedRequired(dest => dest.NomeProjeto, src => src.NomeProjeto)
                .MapTrimmedRequired(dest => dest.SoftwareUtilizado, src => src.SoftwareUtilizado)
                .MapTrimmedRequired(dest => dest.CaminhoPastaServidor, src => src.CaminhoPastaServidor)
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.Revisoes, opt => opt.Ignore())
                .ForMember(dest => dest.RegistosTempo, opt => opt.Ignore());
        }

        private void ConfigureUpdateMap()
        {
            CreateMap<UpdateProjetoDto, Projeto>()
                .MapTrimmedIfProvided(dest => dest.NomeProjeto, src => src.NomeProjeto)
                .MapTrimmedIfProvided(dest => dest.SoftwareUtilizado, src => src.SoftwareUtilizado)
                .MapTrimmedIfProvided(dest => dest.CaminhoPastaServidor, src => src.CaminhoPastaServidor)
                .MapValueIfProvided(dest => dest.TipoProjeto, src => src.TipoProjeto)
                .ForMember(dest => dest.Projeto_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.Revisoes, opt => opt.Ignore())
                .ForMember(dest => dest.RegistosTempo, opt => opt.Ignore());
        }

        private void ConfigureResponseMap()
        {
            CreateMap<Projeto, ResponseProjetoDto>();
        }

        private void ConfigureResponseWithRevisoesMap()
        {
            CreateMap<Projeto, ResponseProjetoWithRevisoesDto>()
                .ForMember(dest => dest.Revisoes, opt => opt.MapFrom(src => src.Revisoes.OrderByDescending(r => r.NumRevisao)));
        }
    }
}
