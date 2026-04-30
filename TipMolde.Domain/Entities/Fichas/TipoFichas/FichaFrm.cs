using TipMolde.Domain.Entities.Fichas.TipoFichas.Linhas;

namespace TipMolde.Domain.Entities.Fichas.TipoFichas
{
    /// <summary>
    /// Representa a ficha FRM de registo de melhorias.
    /// </summary>
    public class FichaFrm : FichaProducao
    {
        /// <summary>
        /// Linhas manuais registadas na ficha FRM.
        /// </summary>
        public ICollection<FichaFrmLinha> Linhas { get; set; } = new List<FichaFrmLinha>();
    }
}
