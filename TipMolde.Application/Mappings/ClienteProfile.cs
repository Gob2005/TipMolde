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
            CreateMap<CreateClienteDto, Cliente>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Trim()))
                .ForMember(dest => dest.NIF, opt => opt.MapFrom(src => src.NIF.Trim()))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.Sigla.Trim()))
                .ForMember(dest => dest.Pais, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Pais) ? null : src.Pais.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Email) ? null : src.Email.Trim()))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Telefone) ? null : src.Telefone.Trim()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.Encomendas, opt => opt.Ignore());

            CreateMap<UpdateClienteDto, Cliente>()
                .ForMember(dest => dest.Nome, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Nome));
                    opt.MapFrom(src => src.Nome!.Trim());
                })
                .ForMember(dest => dest.NIF, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.NIF));
                    opt.MapFrom(src => src.NIF!.Trim());
                })
                .ForMember(dest => dest.Sigla, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Sigla));
                    opt.MapFrom(src => src.Sigla!.Trim());
                })
                .ForMember(dest => dest.Pais, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Pais));
                    opt.MapFrom(src => src.Pais!.Trim());
                })
                .ForMember(dest => dest.Email, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Email));
                    opt.MapFrom(src => src.Email!.Trim());
                })
                .ForMember(dest => dest.Telefone, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Telefone));
                    opt.MapFrom(src => src.Telefone!.Trim());
                })
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Encomendas, opt => opt.Ignore());

            CreateMap<Cliente, ResponseClienteDto>()
                .ForMember(dest => dest.Cliente_id, opt => opt.MapFrom(src => src.Cliente_id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Trim()))
                .ForMember(dest => dest.NIF, opt => opt.MapFrom(src => src.NIF.Trim()))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.Sigla.Trim()))
                .ForMember(dest => dest.Pais, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Pais) ? null : src.Pais.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Email) ? null : src.Email.Trim()))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Telefone) ? null : src.Telefone.Trim()));

            CreateMap<Cliente, ResponseClienteWithEncomendasDto>()
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.Cliente_id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Trim()))
                .ForMember(dest => dest.NIF, opt => opt.MapFrom(src => src.NIF.Trim()))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.Sigla.Trim()))
                .ForMember(dest => dest.Pais, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Pais) ? null : src.Pais.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Email) ? null : src.Email.Trim()))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Telefone) ? null : src.Telefone.Trim()))
                .ForMember(dest => dest.Encomendas, opt => opt.MapFrom(src => src.Encomendas));
        }
    }
}
