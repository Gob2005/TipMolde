using AutoMapper;
using TipMolde.Application.Dtos.EncomendaMoldeDto;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado ao agregado EncomendaMolde.
    /// </summary>
    public class EncomendaMoldeProfile : Profile
    {
        public EncomendaMoldeProfile()
        {
            CreateMap<CreateEncomendaMoldeDto, EncomendaMolde>()
                .ForMember(dest => dest.EncomendaMolde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Encomenda, opt => opt.Ignore())
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.Fichas, opt => opt.Ignore());

            CreateMap<UpdateEncomendaMoldeDto, EncomendaMolde>()
                .ForMember(dest => dest.Quantidade, opt =>
                {
                    opt.Condition(src => src.Quantidade.HasValue);
                    opt.MapFrom(src => src.Quantidade!.Value);
                })
                .ForMember(dest => dest.Prioridade, opt =>
                {
                    opt.Condition(src => src.Prioridade.HasValue);
                    opt.MapFrom(src => src.Prioridade!.Value);
                })
                .ForMember(dest => dest.DataEntregaPrevista, opt =>
                {
                    opt.Condition(src => src.DataEntregaPrevista.HasValue);
                    opt.MapFrom(src => src.DataEntregaPrevista!.Value);
                })
                .ForMember(dest => dest.EncomendaMolde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Encomenda_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Encomenda, opt => opt.Ignore())
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.Fichas, opt => opt.Ignore());

            CreateMap<EncomendaMolde, ResponseEncomendaMoldeDto>()
                .ForMember(dest => dest.NumeroEncomendaCliente, opt => opt.MapFrom(src => src.Encomenda == null ? null : src.Encomenda.NumeroEncomendaCliente))
                .ForMember(dest => dest.NumeroMolde, opt => opt.MapFrom(src => src.Molde == null ? null : src.Molde.Numero));
        }
    }
}
