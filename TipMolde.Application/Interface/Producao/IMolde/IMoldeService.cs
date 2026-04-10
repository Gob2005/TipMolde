using TipMolde.Domain.Entities.Producao;

/// <summary>
/// Serviço de gestão de moldes.
/// </summary>
/// <remarks>
/// CreateAsync foi simplificado: recebe apenas Molde e Specs.
/// A ligação com Encomenda é responsabilidade do IEncomendaMoldeService.
/// Isto respeita SRP e facilita testes unitários.
/// </remarks>
namespace TipMolde.Application.Interface.Producao.IMolde
{
    public interface IMoldeService
    {
        Task<PagedResult<Molde>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Molde?> GetByIdAsync(int id);
        Task<Molde?> GetByIdWithSpecsAsync(int id);
        Task<Molde?> GetByNumeroAsync(string numero);
        Task<IEnumerable<Molde>> GetByEncomendaIdAsync(int encomendaId);

        /// <summary>
        /// Verifica se já existe molde com o número especificado.
        /// </summary>
        Task<bool> ExistsByNumeroAsync(string numero);

        /// <summary>
        /// Cria molde com especificações técnicas numa transação.
        /// </summary>
        /// <remarks>
        /// Ligação com encomenda deve ser feita posteriormente via IEncomendaMoldeService.
        /// </remarks>
        Task<Molde> CreateAsync(Molde molde, EspecificacoesTecnicas specs);

        Task UpdateAsync(Molde molde, EspecificacoesTecnicas? specs);
        Task DeleteAsync(int id);
    }
}
