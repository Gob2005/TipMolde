using TipMolde.Domain.Entities.Fichas.TipoFichas.Linhas;

namespace TipMolde.Domain.Entities.Fichas.TipoFichas
{
    /// <summary>
    /// Representa a ficha FRA de registo de alteracoes.
    /// </summary>
    public class FichaFra : FichaProducao
    {
        /// <summary>
        /// Linhas manuais registadas na ficha FRA.
        /// </summary>
        public ICollection<FichaFraLinha> Linhas { get; set; } = new List<FichaFraLinha>();
    }
}
