using AutoMapper;
using TipMolde.Application.Dtos.PecaDto;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado ao agregado Peca.
    /// </summary>
    /// <remarks>
    /// Centraliza o mapping entre Dtos e entidade para evitar logica dispersa no controller
    /// e reduzir divergencias no contrato HTTP.
    /// </remarks>
    public class PecaProfile : Profile
    {
        /// <summary>
        /// Configura os mapeamentos da feature Peca.
        /// </summary>
        public PecaProfile()
        {
            CreateMap<CreatePecaDto, Peca>()
                .ForMember(dest => dest.Peca_id, opt => opt.Ignore())
                .ForMember(dest => dest.Designacao, opt => opt.MapFrom(src => src.Designacao.Trim()))
                .ForMember(dest => dest.MaterialDesignacao, opt => opt.MapFrom(src => NormalizeOptionalString(src.MaterialDesignacao)))
                .ForMember(dest => dest.Molde_id, opt => opt.MapFrom(src => src.Molde_id))
                .ForMember(dest => dest.Molde, opt => opt.Ignore());

            CreateMap<UpdatePecaDto, Peca>()
                .ForMember(dest => dest.Designacao, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Designacao));
                    opt.MapFrom(src => src.Designacao!.Trim());
                })
                .ForMember(dest => dest.Prioridade, opt =>
                {
                    opt.Condition(src => src.Prioridade.HasValue);
                    opt.MapFrom(src => src.Prioridade!.Value);
                })
                .ForMember(dest => dest.MaterialDesignacao, opt =>
                {
                    opt.Condition(src => src.MaterialDesignacao != null);
                    opt.MapFrom(src => NormalizeOptionalString(src.MaterialDesignacao));
                })
                .ForMember(dest => dest.MaterialRecebido, opt =>
                {
                    opt.Condition(src => src.MaterialRecebido.HasValue);
                    opt.MapFrom(src => src.MaterialRecebido!.Value);
                })
                .ForMember(dest => dest.Peca_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde, opt => opt.Ignore());

            CreateMap<Peca, ResponsePecaDto>()
                .ForMember(dest => dest.PecaId, opt => opt.MapFrom(src => src.Peca_id))
                .ForMember(dest => dest.Molde_id, opt => opt.MapFrom(src => src.Molde_id));
        }

        private static string? NormalizeOptionalString(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
