using TipMolde.Core.Enums;

namespace TipMolde.Core.Models
{
    public class RegistosProducao
    {
        public int Registo_Producao_id { get; set; }
        public EstadoProducao Estado_producao { get; set; }
        public DateTime Data_hora { get; set; }


        public int Fase_id { get; set; }
        public int Operador_id { get; set; }
        public int Peca_id { get; set; }
        public int Maquina_id { get; set; }

        public FasesProducao? Fase { get; set; }
        public User? Operador { get; set; }
        public Peca? Peca { get; set; }
        public Maquina? Maquina { get; set; }
    }
}
