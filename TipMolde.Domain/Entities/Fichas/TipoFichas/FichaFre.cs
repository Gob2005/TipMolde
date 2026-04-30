namespace TipMolde.Domain.Entities.Fichas.TipoFichas
{
    /// <summary>
    /// Representa a ficha FRE usada como contexto documental para entrega ao cliente.
    /// </summary>
    /// <remarks>
    /// O sistema apenas gere o contexto da ficha e a geracao do documento oficial.
    /// O preenchimento funcional da FRE acontece fora da aplicacao e nao e persistido nesta entidade.
    /// </remarks>
    public class FichaFre : FichaProducao
    {
    }
}
