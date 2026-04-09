using TipMolde.Core.Enums;

namespace TipMolde.Core.Models.Producao
{
    /// <summary>
    /// Representa uma fase do processo produtivo (Maquinação, Erosão, Montagem).
    /// Configurável pelo administrador para adaptar o sistema a mudanças no processo.
    /// </summary>
    /// <remarks>
    /// O nome é único e validado por índice na base de dados.
    /// Novas fases podem ser adicionadas sem alteração de código.
    /// </remarks>
    public class FasesProducao
    {
        public int Fases_producao_id { get; set; }

        /// <summary>
        /// Nome da fase extraído de enum para garantir valores válidos.
        /// Alternativa a magic strings no código.
        /// </summary>
        public required Nome_fases Nome { get; set; }

        /// <summary>
        /// Descrição textual da fase, útil para novos colaboradores.
        /// Pode incluir instruções resumidas ou referências a procedimentos.
        /// </summary>
        public string? Descricao { get; set; }
    }
}