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
            CreateMap<CreateEncomendaDto, Encomenda>()
                .ForMember(dest => dest.Cliente_id, opt => opt.MapFrom(src => src.Cliente_id))
                .ForMember(dest => dest.NumeroEncomendaCliente, opt => opt.MapFrom(src => src.NumeroEncomendaCliente.Trim()))
                .ForMember(dest => dest.NumeroProjetoCliente, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.NumeroProjetoCliente) ? null : src.NumeroProjetoCliente.Trim()))
                .ForMember(dest => dest.NomeServicoCliente, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.NomeServicoCliente) ? null : src.NomeServicoCliente.Trim()))
                .ForMember(dest => dest.NomeResponsavelCliente, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.NomeResponsavelCliente) ? null : src.NomeResponsavelCliente.Trim()))
                .ForMember(dest => dest.Encomenda_id, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.Ignore())
                .ForMember(dest => dest.DataRegisto, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.EncomendasMoldes, opt => opt.Ignore());

            CreateMap<UpdateEncomendaDto, Encomenda>()
                .ForMember(dest => dest.NumeroEncomendaCliente, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.NumeroEncomendaCliente));
                    opt.MapFrom(src => src.NumeroEncomendaCliente!.Trim());
                })
                .ForMember(dest => dest.NumeroProjetoCliente, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.NumeroProjetoCliente));
                    opt.MapFrom(src => src.NumeroProjetoCliente!.Trim());
                })
                .ForMember(dest => dest.NomeServicoCliente, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.NomeServicoCliente));
                    opt.MapFrom(src => src.NomeServicoCliente!.Trim());
                })
                .ForMember(dest => dest.NomeResponsavelCliente, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.NomeResponsavelCliente));
                    opt.MapFrom(src => src.NomeResponsavelCliente!.Trim());
                })
                .ForMember(dest => dest.Encomenda_id, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.Ignore())
                .ForMember(dest => dest.DataRegisto, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente_id, opt => opt.Ignore())
                .ForMember(dest => dest.EncomendasMoldes, opt => opt.Ignore());

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

            CreateMap<Encomenda, ResponseEncomendaDto>()
                .ForMember(dest => dest.Encomenda_id, opt => opt.MapFrom(src => src.Encomenda_id))
                .ForMember(dest => dest.NumeroEncomendaCliente, opt => opt.MapFrom(src => src.NumeroEncomendaCliente.Trim()))
                .ForMember(dest => dest.NumeroProjetoCliente, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.NumeroProjetoCliente) ? null : src.NumeroProjetoCliente.Trim()))
                .ForMember(dest => dest.NomeServicoCliente, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.NomeServicoCliente) ? null : src.NomeServicoCliente.Trim()))
                .ForMember(dest => dest.NomeResponsavelCliente, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.NomeResponsavelCliente) ? null : src.NomeResponsavelCliente.Trim()))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
                .ForMember(dest => dest.DataRegisto, opt => opt.MapFrom(src => src.DataRegisto))
                .ForMember(dest => dest.Cliente_id, opt => opt.MapFrom(src => src.Cliente_id))
                .ForMember(dest => dest.NomeCliente, opt => opt.MapFrom(src => src.Cliente == null ? null : src.Cliente.Nome));
        }
    }
}
