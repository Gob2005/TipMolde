using AutoMapper;
using TipMolde.Application.Dtos.EncomendaDto;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado ao agregado Encomenda.
    /// </summary>
    public class EncomendaProfile : Profile
    {
        /// <summary>
        /// Configura mapeamentos entre entidade Encomenda e Dtos.
        /// </summary>
        public EncomendaProfile()
        {
            ConfigureCreateMap();
            ConfigureUpdateMap();
            ConfigureEstadoUpdateMap();
            ConfigureResponseMap();
        }

        private void ConfigureCreateMap()
        {
            CreateMap<CreateEncomendaDto, Encomenda>()
                .ForMember(dest => dest.Cliente_id, opt => opt.MapFrom(src => src.Cliente_id))
                .MapTrimmedRequired(dest => dest.NumeroEncomendaCliente, src => src.NumeroEncomendaCliente)
                .MapTrimmedOptional(dest => dest.NumeroProjetoCliente, src => src.NumeroProjetoCliente)
                .MapTrimmedOptional(dest => dest.NomeServicoCliente, src => src.NomeServicoCliente)
                .MapTrimmedOptional(dest => dest.NomeResponsavelCliente, src => src.NomeResponsavelCliente)
                .ForMember(dest => dest.Encomenda_id, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.Ignore())
                .ForMember(dest => dest.DataRegisto, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.EncomendasMoldes, opt => opt.Ignore());
        }

        private void ConfigureUpdateMap()
        {
            CreateMap<UpdateEncomendaDto, Encomenda>()
                .MapTrimmedIfProvided(dest => dest.NumeroEncomendaCliente, src => src.NumeroEncomendaCliente)
                .MapTrimmedIfProvided(dest => dest.NumeroProjetoCliente, src => src.NumeroProjetoCliente)
                .MapTrimmedIfProvided(dest => dest.NomeServicoCliente, src => src.NomeServicoCliente)
                .MapTrimmedIfProvided(dest => dest.NomeResponsavelCliente, src => src.NomeResponsavelCliente)
                .ForMember(dest => dest.Encomenda_id, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.Ignore())
                .ForMember(dest => dest.DataRegisto, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.EncomendasMoldes, opt => opt.Ignore());
        }

        private void ConfigureEstadoUpdateMap()
        {
            CreateMap<UpdateEstadoEncomendaDto, Encomenda>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
                .ForMember(dest => dest.Encomenda_id, opt => opt.Ignore())
                .ForMember(dest => dest.DataRegisto, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.EncomendasMoldes, opt => opt.Ignore())
                .ForMember(dest => dest.NumeroEncomendaCliente, opt => opt.Ignore())
                .ForMember(dest => dest.NumeroProjetoCliente, opt => opt.Ignore())
                .ForMember(dest => dest.NomeServicoCliente, opt => opt.Ignore())
                .ForMember(dest => dest.NomeResponsavelCliente, opt => opt.Ignore());
        }

        private void ConfigureResponseMap()
        {
            CreateMap<Encomenda, ResponseEncomendaDto>()
                .ForMember(dest => dest.Encomenda_id, opt => opt.MapFrom(src => src.Encomenda_id))
                .MapTrimmedRequired(dest => dest.NumeroEncomendaCliente, src => src.NumeroEncomendaCliente)
                .MapTrimmedOptional(dest => dest.NumeroProjetoCliente, src => src.NumeroProjetoCliente)
                .MapTrimmedOptional(dest => dest.NomeServicoCliente, src => src.NomeServicoCliente)
                .MapTrimmedOptional(dest => dest.NomeResponsavelCliente, src => src.NomeResponsavelCliente)
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
                .ForMember(dest => dest.DataRegisto, opt => opt.MapFrom(src => src.DataRegisto))
                .ForMember(dest => dest.Cliente_id, opt => opt.MapFrom(src => src.Cliente_id))
                .ForMember(dest => dest.NomeCliente, opt => opt.MapFrom(src => MappingProfileExtensions.GetOptionalValue(src.Cliente, cliente => cliente.Nome)));
        }
    }
}
