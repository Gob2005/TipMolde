using AutoMapper;
using TipMolde.Application.Dtos.MoldeDto;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Profile AutoMapper dedicado ao agregado Molde.
    /// </summary>
    /// <remarks>
    /// Centraliza o mapping entre Dtos e entidades para evitar logica dispersa no controller.
    /// </remarks>
    public class MoldeProfile : Profile
    {
        /// <summary>
        /// Configura os mapeamentos da feature Molde.
        /// </summary>
        public MoldeProfile()
        {
            CreateMap<CreateMoldeDto, Molde>()
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Numero.Trim()))
                .ForMember(dest => dest.NumeroMoldeCliente, opt => opt.MapFrom(src => NormalizeOptionalString(src.NumeroMoldeCliente)))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => NormalizeOptionalString(src.Nome)))
                .ForMember(dest => dest.ImagemCapaPath, opt => opt.MapFrom(src => NormalizeOptionalString(src.ImagemCapaPath)))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => NormalizeOptionalString(src.Descricao)))
                .ForMember(dest => dest.Especificacoes, opt => opt.Ignore())
                .ForMember(dest => dest.Pecas, opt => opt.Ignore())
                .ForMember(dest => dest.EncomendasMoldes, opt => opt.Ignore());

            CreateMap<CreateMoldeDto, EspecificacoesTecnicas>()
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.TipoInjecao, opt => opt.MapFrom(src => NormalizeOptionalString(src.TipoInjecao)))
                .ForMember(dest => dest.SistemaInjecao, opt => opt.MapFrom(src => NormalizeOptionalString(src.SistemaInjecao)))
                .ForMember(dest => dest.AcabamentoPeca, opt => opt.MapFrom(src => NormalizeOptionalString(src.AcabamentoPeca)))
                .ForMember(dest => dest.MaterialMacho, opt => opt.MapFrom(src => NormalizeOptionalString(src.MaterialMacho)))
                .ForMember(dest => dest.MaterialCavidade, opt => opt.MapFrom(src => NormalizeOptionalString(src.MaterialCavidade)))
                .ForMember(dest => dest.MaterialMovimentos, opt => opt.MapFrom(src => NormalizeOptionalString(src.MaterialMovimentos)))
                .ForMember(dest => dest.MaterialInjecao, opt => opt.MapFrom(src => NormalizeOptionalString(src.MaterialInjecao)))
                .ForMember(dest => dest.LadoFixo, opt => opt.Ignore())
                .ForMember(dest => dest.LadoMovel, opt => opt.Ignore());

            CreateMap<CreateMoldeDto, EncomendaMolde>()
                .ForMember(dest => dest.EncomendaMolde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Encomenda_id, opt => opt.MapFrom(src => src.EncomendaId))
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Encomenda, opt => opt.Ignore())
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.Fichas, opt => opt.Ignore());

            CreateMap<UpdateMoldeDto, Molde>()
                .ForMember(dest => dest.Numero, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Numero));
                    opt.MapFrom(src => src.Numero!.Trim());
                })
                .ForMember(dest => dest.NumeroMoldeCliente, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.NumeroMoldeCliente));
                    opt.MapFrom(src => src.NumeroMoldeCliente!.Trim());
                })
                .ForMember(dest => dest.Nome, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Nome));
                    opt.MapFrom(src => src.Nome!.Trim());
                })
                .ForMember(dest => dest.ImagemCapaPath, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.ImagemCapaPath));
                    opt.MapFrom(src => src.ImagemCapaPath!.Trim());
                })
                .ForMember(dest => dest.Descricao, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Descricao));
                    opt.MapFrom(src => src.Descricao!.Trim());
                })
                .ForMember(dest => dest.Numero_cavidades, opt =>
                {
                    opt.Condition(src => src.Numero_cavidades.HasValue);
                    opt.MapFrom(src => src.Numero_cavidades!.Value);
                })
                .ForMember(dest => dest.TipoPedido, opt =>
                {
                    opt.Condition(src => src.TipoPedido.HasValue);
                    opt.MapFrom(src => src.TipoPedido!.Value);
                })
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Especificacoes, opt => opt.Ignore())
                .ForMember(dest => dest.Pecas, opt => opt.Ignore())
                .ForMember(dest => dest.EncomendasMoldes, opt => opt.Ignore());

            CreateMap<UpdateMoldeDto, EspecificacoesTecnicas>()
                .ForMember(dest => dest.Largura, opt =>
                {
                    opt.Condition(src => src.Largura.HasValue);
                    opt.MapFrom(src => src.Largura!.Value);
                })
                .ForMember(dest => dest.Comprimento, opt =>
                {
                    opt.Condition(src => src.Comprimento.HasValue);
                    opt.MapFrom(src => src.Comprimento!.Value);
                })
                .ForMember(dest => dest.Altura, opt =>
                {
                    opt.Condition(src => src.Altura.HasValue);
                    opt.MapFrom(src => src.Altura!.Value);
                })
                .ForMember(dest => dest.PesoEstimado, opt =>
                {
                    opt.Condition(src => src.PesoEstimado.HasValue);
                    opt.MapFrom(src => src.PesoEstimado!.Value);
                })
                .ForMember(dest => dest.TipoInjecao, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.TipoInjecao));
                    opt.MapFrom(src => src.TipoInjecao!.Trim());
                })
                .ForMember(dest => dest.SistemaInjecao, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.SistemaInjecao));
                    opt.MapFrom(src => src.SistemaInjecao!.Trim());
                })
                .ForMember(dest => dest.Contracao, opt =>
                {
                    opt.Condition(src => src.Contracao.HasValue);
                    opt.MapFrom(src => src.Contracao!.Value);
                })
                .ForMember(dest => dest.AcabamentoPeca, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.AcabamentoPeca));
                    opt.MapFrom(src => src.AcabamentoPeca!.Trim());
                })
                .ForMember(dest => dest.Cor, opt =>
                {
                    opt.Condition(src => src.Cor.HasValue);
                    opt.MapFrom(src => src.Cor!.Value);
                })
                .ForMember(dest => dest.MaterialMacho, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.MaterialMacho));
                    opt.MapFrom(src => src.MaterialMacho!.Trim());
                })
                .ForMember(dest => dest.MaterialCavidade, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.MaterialCavidade));
                    opt.MapFrom(src => src.MaterialCavidade!.Trim());
                })
                .ForMember(dest => dest.MaterialMovimentos, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.MaterialMovimentos));
                    opt.MapFrom(src => src.MaterialMovimentos!.Trim());
                })
                .ForMember(dest => dest.MaterialInjecao, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.MaterialInjecao));
                    opt.MapFrom(src => src.MaterialInjecao!.Trim());
                })
                .ForMember(dest => dest.Molde_id, opt => opt.Ignore())
                .ForMember(dest => dest.Molde, opt => opt.Ignore())
                .ForMember(dest => dest.LadoFixo, opt => opt.Ignore())
                .ForMember(dest => dest.LadoMovel, opt => opt.Ignore());

            CreateMap<Molde, ResponseMoldeDto>()
                .ForMember(dest => dest.MoldeId, opt => opt.MapFrom(src => src.Molde_id))
                .ForMember(dest => dest.NumeroMoldeCliente, opt => opt.MapFrom(src => src.NumeroMoldeCliente))
                .ForMember(dest => dest.Largura, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.Largura))
                .ForMember(dest => dest.Comprimento, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.Comprimento))
                .ForMember(dest => dest.Altura, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.Altura))
                .ForMember(dest => dest.PesoEstimado, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.PesoEstimado))
                .ForMember(dest => dest.TipoInjecao, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.TipoInjecao))
                .ForMember(dest => dest.SistemaInjecao, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.SistemaInjecao))
                .ForMember(dest => dest.Contracao, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.Contracao))
                .ForMember(dest => dest.AcabamentoPeca, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.AcabamentoPeca))
                .ForMember(dest => dest.Cor, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.Cor))
                .ForMember(dest => dest.MaterialMacho, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.MaterialMacho))
                .ForMember(dest => dest.MaterialCavidade, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.MaterialCavidade))
                .ForMember(dest => dest.MaterialMovimentos, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.MaterialMovimentos))
                .ForMember(dest => dest.MaterialInjecao, opt => opt.MapFrom(src => src.Especificacoes == null ? null : src.Especificacoes.MaterialInjecao));
        }

        private static string? NormalizeOptionalString(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
