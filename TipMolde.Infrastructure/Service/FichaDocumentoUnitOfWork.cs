using TipMolde.Application.Interface.Fichas.IFichaDocumento;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Service
{
    /// <summary>
    /// Implementa o boundary transacional dos fluxos documentais das fichas com EF Core.
    /// </summary>
    public class FichaDocumentoUnitOfWork : IFichaDocumentoUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// Construtor de FichaDocumentoUnitOfWork.
        /// </summary>
        /// <param name="dbContext">Contexto EF Core usado para abrir e fechar transacoes documentais.</param>
        public FichaDocumentoUnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Executa um fluxo documental dentro de uma transacao atomica.
        /// </summary>
        /// <typeparam name="T">Tipo do resultado devolvido pelo fluxo.</typeparam>
        /// <param name="action">Operacao documental a executar de forma atomica.</param>
        /// <returns>Resultado devolvido pela operacao transacional.</returns>
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
