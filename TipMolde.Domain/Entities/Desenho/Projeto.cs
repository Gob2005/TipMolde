using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Desenho
{
    /// <summary>
    /// Representa um projeto de desenho associado a um molde.
    /// </summary>
    /// <remarks>
    /// O agregado guarda a identificacao funcional do projeto, o software utilizado,
    /// o tipo de projeto e o caminho da pasta tecnica no servidor.
    /// </remarks>
    public class Projeto
    {
        public int Projeto_id { get; set; }
        public required string NomeProjeto { get; set; }
        public required string SoftwareUtilizado { get; set; }
        public TipoProjeto TipoProjeto { get; set; }
        public required string CaminhoPastaServidor { get; set; }

        public int Molde_id { get; set; }
        public Molde? Molde { get; set; }

        public ICollection<Revisao> Revisoes { get; set; } = new List<Revisao>();
        public ICollection<RegistoTempoProjeto> RegistosTempo { get; set; } = new List<RegistoTempoProjeto>();
    }
}
