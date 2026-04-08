using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.ProjetoDTO
{
    public class ResponseProjetoDTO
    {
        public int Projeto_id { get; set; }
        public string NomeProjeto { get; set; } = string.Empty;
        public string SoftwareUtilizado { get; set; } = string.Empty;
        public TipoProjeto TipoProjeto { get; set; }
        public string CaminhoPastaServidor { get; set; } = string.Empty;
        public int Molde_id { get; set; }
    }
}
