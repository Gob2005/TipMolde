using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.FasesProducaoDto
{
    /// <summary>
    /// Representa a resposta publica da feature FasesProducao.
    /// </summary>
    public class ResponseFasesProducaoDto
    {
        /// <summary>
        /// Identificador interno da fase.
        /// </summary>
        public int FasesProducao_id { get; set; }

        /// <summary>
        /// Nome funcional da fase.
        /// </summary>
        public Nome_fases Nome { get; set; }

        /// <summary>
        /// Descricao funcional da fase.
        /// </summary>
        public string? Descricao { get; set; }
    }
}
