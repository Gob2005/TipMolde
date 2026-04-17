using AutoMapper;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<CreateClienteDTO, Cliente>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Trim()))
                .ForMember(dest => dest.NIF, opt => opt.MapFrom(src => src.NIF.Trim()))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.Sigla.Trim()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.Encomendas, opt => opt.Ignore());

            CreateMap<UpdateClienteDTO, Cliente>()
                .ForMember(dest => dest.Nome, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Nome)))
                .ForMember(dest => dest.NIF, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.NIF)))
                .ForMember(dest => dest.Sigla, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Sigla)))
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Encomendas, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Cliente, ResponseClienteDTO>()
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.Cliente_id));
        }
    }
}
