using AutoMapper;
using TipMolde.Application.Dtos.FornecedorDto;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Mappings
{
    /// <summary>
    /// Define os mapeamentos AutoMapper do agregado Fornecedor.
    /// </summary>
    /// <remarks>
    /// Centraliza conversoes entre Dtos de fornecedor e a entidade de dominio, incluindo normalizacao de campos textuais.
    /// </remarks>
    public class FornecedorProfile : Profile
    {
        /// <summary>
        /// Construtor de FornecedorProfile.
        /// </summary>
        public FornecedorProfile()
        {
            ConfigureCreateMap();
            ConfigureUpdateMap();
            ConfigureResponseMap();
        }

        private void ConfigureCreateMap()
        {
            CreateMap<CreateFornecedorDto, Fornecedor>()
                .MapTrimmedRequired(dest => dest.Nome, src => src.Nome)
                .MapTrimmedRequired(dest => dest.NIF, src => src.NIF)
                .MapTrimmedOptional(dest => dest.Morada, src => src.Morada)
                .MapTrimmedOptional(dest => dest.Email, src => src.Email)
                .MapTrimmedOptional(dest => dest.Telefone, src => src.Telefone)
                .ForMember(dest => dest.Fornecedor_id, opt => opt.Ignore());
        }

        private void ConfigureUpdateMap()
        {
            CreateMap<UpdateFornecedorDto, Fornecedor>()
                .MapTrimmedIfProvided(dest => dest.Nome, src => src.Nome)
                .MapTrimmedIfProvided(dest => dest.NIF, src => src.NIF)
                .MapTrimmedIfProvided(dest => dest.Morada, src => src.Morada)
                .MapTrimmedIfProvided(dest => dest.Email, src => src.Email)
                .MapTrimmedIfProvided(dest => dest.Telefone, src => src.Telefone)
                .ForMember(dest => dest.Fornecedor_id, opt => opt.Ignore());
        }

        private void ConfigureResponseMap()
        {
            CreateMap<Fornecedor, ResponseFornecedorDto>()
                .ForMember(dest => dest.FornecedorId, opt => opt.MapFrom(src => src.Fornecedor_id))
                .MapTrimmedRequired(dest => dest.Nome, src => src.Nome)
                .MapTrimmedRequired(dest => dest.NIF, src => src.NIF)
                .MapTrimmedOptional(dest => dest.Morada, src => src.Morada)
                .MapTrimmedOptional(dest => dest.Email, src => src.Email)
                .MapTrimmedOptional(dest => dest.Telefone, src => src.Telefone);
        }
    }
}
