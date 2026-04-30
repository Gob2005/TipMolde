using TipMolde.Domain.Entities.Fichas.TipoFichas.Linhas;

namespace TipMolde.Domain.Entities.Fichas.TipoFichas
{
    /// <summary>
    /// Representa a ficha FOP de ocorrencias na producao.
    /// </summary>
    public class FichaFop : FichaProducao
    {
        /// <summary>
        /// Linhas manuais registadas na ficha FOP.
        /// </summary>
        public ICollection<FichaFopLinha> Linhas { get; set; } = new List<FichaFopLinha>();
    }
}
