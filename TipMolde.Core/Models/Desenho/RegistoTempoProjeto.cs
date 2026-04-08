using TipMolde.Core.Enums;

namespace TipMolde.Core.Models.Desenho
{
    public class RegistoTempoProjeto
    {
        public int Registo_Tempo_Projeto_id { get; set; }
        public EstadoTempoProjeto Estado_tempo { get; set; } = EstadoTempoProjeto.INICIADO;
        public DateTime Data_hora { get; set; }

        public int Projeto_id { get; set; }
        public Projeto? Projeto { get; set; }

        public int Autor_id { get; set; }
        public User? Autor { get; set; }
    }
}
