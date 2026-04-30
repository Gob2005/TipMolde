namespace TipMolde.Application.Interface.Fichas.IFichaDocumento
{
    /// <summary>
    /// Abstrai o boundary transacional usado pelos casos de uso documentais das fichas.
    /// </summary>
    public interface IFichaDocumentoUnitOfWork
    {
        /// <summary>
        /// Executa um fluxo documental dentro de uma transacao atomica.
        /// </summary>
        /// <typeparam name="T">Tipo do resultado devolvido pelo fluxo.</typeparam>
        /// <param name="action">Operacao documental a executar de forma atomica.</param>
        /// <returns>Resultado devolvido pela operacao transacional.</returns>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
