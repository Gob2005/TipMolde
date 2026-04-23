using TipMolde.Application.DTOs.RevisaoDTO;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.ProjetoDTO
{
    /// <summary>
    /// Representa o detalhe de um projeto com a colecao de revisoes associadas.
    /// </summary>
    /// <remarks>
    /// Este DTO evita expor diretamente o grafo de dominio nos cenarios de leitura enriquecida.
    /// </remarks>
    public class ResponseProjetoWithRevisoesDTO
    {
        public int Projeto_id { get; set; }
        public string NomeProjeto { get; set; } = string.Empty;
        public string SoftwareUtilizado { get; set; } = string.Empty;
        public TipoProjeto TipoProjeto { get; set; }
        public string CaminhoPastaServidor { get; set; } = string.Empty;
        public int Molde_id { get; set; }
        public IEnumerable<ResponseRevisaoDTO> Revisoes { get; set; } = new List<ResponseRevisaoDTO>();
    }
}
