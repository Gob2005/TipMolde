using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Producao
{
    /// <summary>
    /// Representa uma fase configuravel do processo produtivo.
    /// </summary>
    /// <remarks>
    /// O nome da fase e gerido pelo administrador e deve permanecer unico.
    /// Uma fase nao deve ser removida enquanto existir pelo menos uma maquina associada.
    /// </remarks>
    public class FasesProducao
    {
        /// <summary>
        /// Identificador interno da fase de producao.
        /// </summary>
        public int Fases_producao_id { get; set; }

        /// <summary>
        /// Nome funcional da fase de producao.
        /// </summary>
        public required Nome_fases Nome { get; set; }

        /// <summary>
        /// Descricao funcional da fase para apoio a utilizadores e administracao.
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Maquinas atualmente associadas a esta fase.
        /// </summary>
        public ICollection<Maquina> MaquinasDedicadas { get; set; } = [];
    }
}
