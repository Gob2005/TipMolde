namespace TipMolde.Application.Exceptions
{
    /// <summary>
    /// Representa conflito de regra de negocio (HTTP 409).
    /// </summary>
    public class BusinessConflictException : Exception
    {
        /// <summary>
        /// Construtor da excecao de conflito de negocio.
        /// </summary>
        /// <param name="message">Mensagem descritiva do conflito.</param>
        public BusinessConflictException(string message) : base(message)
        {
        }
    }
}
