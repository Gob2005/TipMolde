using AutoMapper;
using TipMolde.Application.Dtos.PedidoMaterialDto;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Define os mapeamentos AutoMapper do agregado PedidoMaterial.
    /// </summary>
    /// <remarks>
    /// Centraliza o mapping entre Dtos de pedido de material e entidades de dominio,
    /// removendo transformacoes espalhadas pelo controller.
    /// </remarks>
    public class PedidoMaterialProfile : Profile
    {
        /// <summary>
        /// Construtor de PedidoMaterialProfile.
        /// </summary>
        public PedidoMaterialProfile()
        {
            ConfigureItemCreateMap();
            ConfigurePedidoCreateMap();
            ConfigureItemResponseMap();
            ConfigurePedidoResponseMap();
        }

        private void ConfigureItemCreateMap()
        {
            CreateMap<CreateItemPedidoMaterialDto, ItemPedidoMaterial>()
                .ForMember(dest => dest.ItemPedidoMaterial_id, opt => opt.Ignore())
                .ForMember(dest => dest.PedidoMaterial_id, opt => opt.Ignore())
                .ForMember(dest => dest.PedidoMaterial, opt => opt.Ignore())
                .ForMember(dest => dest.Peca, opt => opt.Ignore());
        }

        private void ConfigurePedidoCreateMap()
        {
            CreateMap<CreatePedidoMaterialDto, PedidoMaterial>()
                .ForMember(dest => dest.PedidoMaterial_id, opt => opt.Ignore())
                .ForMember(dest => dest.DataPedido, opt => opt.Ignore())
                .ForMember(dest => dest.DataRececao, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.Ignore())
                .ForMember(dest => dest.Fornecedor, opt => opt.Ignore())
                .ForMember(dest => dest.UserConferente_id, opt => opt.Ignore())
                .ForMember(dest => dest.Conferente, opt => opt.Ignore())
                .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens));
        }

        private void ConfigureItemResponseMap()
        {
            CreateMap<ItemPedidoMaterial, ResponseItemPedidoMaterialDto>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemPedidoMaterial_id))
                .ForMember(dest => dest.PecaId, opt => opt.MapFrom(src => src.Peca_id));
        }

        private void ConfigurePedidoResponseMap()
        {
            CreateMap<PedidoMaterial, ResponsePedidoMaterialDto>()
                .ForMember(dest => dest.PedidoMaterialId, opt => opt.MapFrom(src => src.PedidoMaterial_id))
                .ForMember(dest => dest.FornecedorId, opt => opt.MapFrom(src => src.Fornecedor_id))
                .ForMember(dest => dest.UserConferenteId, opt => opt.MapFrom(src => src.UserConferente_id))
                .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens));
        }
    }
}
