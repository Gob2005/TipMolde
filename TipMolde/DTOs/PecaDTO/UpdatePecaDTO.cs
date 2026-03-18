using TipMolde.Core.Models;

namespace TipMolde.API.DTOs.PecaDTO
{
    public class UpdatePecaDTO
    {
        public int Numero_peca { get; set; }
        public int Prioridade { get; set; }
        public string? Descricao { get; set; }
        public int Molde_id { get; set; }

    }
}
