using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Interface.Desenho.IRevisao
{
    public interface IRevisaoService
    {
        Task<IEnumerable<Revisao>> GetByProjetoIdAsync(int projetoId);
        Task<Revisao?> GetByIdAsync(int id);
        Task<Revisao> CreateAsync(Revisao revisao);
        Task UpdateRespostaClienteAsync(int revisaoId, bool aprovado, string? feedbackTexto, string? feedbackImagemPath);
        Task DeleteAsync(int id);
    }
}
