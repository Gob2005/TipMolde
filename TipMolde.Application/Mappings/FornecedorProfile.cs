using AutoMapper;
using TipMolde.Application.DTOs.FornecedorDTO;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Define os mapeamentos AutoMapper do agregado Fornecedor.
    /// </summary>
    /// <remarks>
    /// Centraliza conversoes entre DTOs de fornecedor e a entidade de dominio, incluindo normalizacao de campos textuais.
    /// </remarks>
    public class FornecedorProfile : Profile
    {
        /// <summary>
        /// Construtor de FornecedorProfile.
        /// </summary>
        public FornecedorProfile()
        {
            CreateMap<CreateFornecedorDTO, Fornecedor>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Trim()))
                .ForMember(dest => dest.NIF, opt => opt.MapFrom(src => src.NIF.Trim()))
                .ForMember(dest => dest.Morada, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Morada) ? null : src.Morada.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Email) ? null : src.Email.Trim()))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Telefone) ? null : src.Telefone.Trim()))
                .ForMember(dest => dest.Fornecedor_id, opt => opt.Ignore());

            CreateMap<UpdateFornecedorDTO, Fornecedor>()
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
                .ForMember(dest => dest.Morada, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Morada));
                    opt.MapFrom(src => src.Morada!.Trim());
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
                .ForMember(dest => dest.Fornecedor_id, opt => opt.Ignore());

            CreateMap<Fornecedor, ResponseFornecedorDTO>()
                .ForMember(dest => dest.FornecedorId, opt => opt.MapFrom(src => src.Fornecedor_id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Trim()))
                .ForMember(dest => dest.NIF, opt => opt.MapFrom(src => src.NIF.Trim()))
                .ForMember(dest => dest.Morada, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Morada) ? null : src.Morada.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Email) ? null : src.Email.Trim()))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Telefone) ? null : src.Telefone.Trim()));
        }
    }
}
