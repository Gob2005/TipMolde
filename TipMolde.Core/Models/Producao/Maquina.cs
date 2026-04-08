using TipMolde.Core.Enums;

namespace TipMolde.Core.Models.Producao
{
    public class Maquina
    {
        public int Maquina_id { get; set; }
        public int Numero { get; set; }
        public required string NomeModelo { get; set; }
        public string? IpAddress { get; set; }
        public EstadoMaquina Estado { get; set; } = EstadoMaquina.DISPONIVEL;

        public int FaseDedicada_id { get; set; }
        public FasesProducao? FaseDedicada { get; set; }
    }

}
