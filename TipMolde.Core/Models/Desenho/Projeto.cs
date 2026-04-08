using TipMolde.Core.Enums;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Models.Desenho
{
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
