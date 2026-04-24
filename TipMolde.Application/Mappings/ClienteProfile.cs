using AutoMapper;
using TipMolde.Application.Dtos.ClienteDto;
using TipMolde.Application.Dtos.EncomendaDto;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Define os mapeamentos AutoMapper do agregado Cliente.
    /// </summary>
    /// <remarks>
    /// Centraliza conversoes entre Dtos de cliente e a entidade de dominio, incluindo normalizacao de campos textuais.
    /// </remarks>
    public class ClienteProfile : Profile
    {
        /// <summary>
        /// Construtor de ClienteProfile.
        /// </summary>
        public ClienteProfile()
        {
            ConfigureCreateMap();
            ConfigureUpdateMap();
            ConfigureResponseMap();
            ConfigureResponseWithEncomendasMap();
        }

        private void ConfigureCreateMap()
        {
            CreateMap<CreateClienteDto, Cliente>()
                .MapTrimmedRequired(dest => dest.Nome, src => src.Nome)
                .MapTrimmedRequired(dest => dest.NIF, src => src.NIF)
                .MapTrimmedRequired(dest => dest.Sigla, src => src.Sigla)
                .MapTrimmedOptional(dest => dest.Pais, src => src.Pais)
                .MapTrimmedOptional(dest => dest.Email, src => src.Email)
                .MapTrimmedOptional(dest => dest.Telefone, src => src.Telefone)
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.Encomendas, opt => opt.Ignore());
        }

        private void ConfigureUpdateMap()
        {
            CreateMap<UpdateClienteDto, Cliente>()
                .MapTrimmedIfProvided(dest => dest.Nome, src => src.Nome)
                .MapTrimmedIfProvided(dest => dest.NIF, src => src.NIF)
                .MapTrimmedIfProvided(dest => dest.Sigla, src => src.Sigla)
                .MapTrimmedIfProvided(dest => dest.Pais, src => src.Pais)
                .MapTrimmedIfProvided(dest => dest.Email, src => src.Email)
                .MapTrimmedIfProvided(dest => dest.Telefone, src => src.Telefone)
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Encomendas, opt => opt.Ignore());
        }

        private void ConfigureResponseMap()
        {
            CreateMap<Cliente, ResponseClienteDto>()
                .ForMember(dest => dest.Cliente_id, opt => opt.MapFrom(src => src.Cliente_id))
                .MapTrimmedRequired(dest => dest.Nome, src => src.Nome)
                .MapTrimmedRequired(dest => dest.NIF, src => src.NIF)
                .MapTrimmedRequired(dest => dest.Sigla, src => src.Sigla)
                .MapTrimmedOptional(dest => dest.Pais, src => src.Pais)
                .MapTrimmedOptional(dest => dest.Email, src => src.Email)
                .MapTrimmedOptional(dest => dest.Telefone, src => src.Telefone);
        }

        private void ConfigureResponseWithEncomendasMap()
        {
            CreateMap<Cliente, ResponseClienteWithEncomendasDto>()
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.Cliente_id))
                .MapTrimmedRequired(dest => dest.Nome, src => src.Nome)
                .MapTrimmedRequired(dest => dest.NIF, src => src.NIF)
                .MapTrimmedRequired(dest => dest.Sigla, src => src.Sigla)
                .MapTrimmedOptional(dest => dest.Pais, src => src.Pais)
                .MapTrimmedOptional(dest => dest.Email, src => src.Email)
                .MapTrimmedOptional(dest => dest.Telefone, src => src.Telefone)
                .ForMember(dest => dest.Encomendas, opt => opt.MapFrom(src => src.Encomendas));
        }
    }
}
