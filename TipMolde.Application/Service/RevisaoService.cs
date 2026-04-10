using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Application.Interface.Desenho.IRevisao;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Infrastructure.Service
{
    public class RevisaoService : IRevisaoService
    {
        private readonly IRevisaoRepository _revisaoRepository;
        private readonly IProjetoRepository _projetoRepository;

        public RevisaoService(IRevisaoRepository revisaoRepository, IProjetoRepository projetoRepository)
        {
            _revisaoRepository = revisaoRepository;
            _projetoRepository = projetoRepository;
        }

        public Task<IEnumerable<Revisao>> GetByProjetoIdAsync(int projetoId) => _revisaoRepository.GetByProjetoIdAsync(projetoId);

        public Task<Revisao?> GetByIdAsync(int id) => _revisaoRepository.GetByIdAsync(id);

        public async Task<Revisao> CreateAsync(Revisao revisao)
        {
            var projeto = await _projetoRepository.GetByIdAsync(revisao.Projeto_id);
            if (projeto == null)
                throw new KeyNotFoundException($"Projeto com ID {revisao.Projeto_id} nao encontrado.");

            if (string.IsNullOrWhiteSpace(revisao.DescricaoAlteracoes))
                throw new ArgumentException("Descricao das alteracoes e obrigatoria.");

            revisao.NumRevisao = await _revisaoRepository.GetNextNumeroRevisaoAsync(revisao.Projeto_id);
            revisao.DataEnvioCliente = DateTime.UtcNow;

            await _revisaoRepository.AddAsync(revisao);
            return revisao;
        }

        public async Task UpdateRespostaClienteAsync(int revisaoId, bool aprovado, string? feedbackTexto, string? feedbackImagemPath)
        {
            var existing = await _revisaoRepository.GetByIdAsync(revisaoId);
            if (existing == null)
                throw new KeyNotFoundException($"Revisao com ID {revisaoId} nao encontrada.");

            existing.Aprovado = aprovado;
            existing.DataResposta = DateTime.UtcNow;
            existing.FeedbackTexto = feedbackTexto;
            existing.FeedbackImagemPath = feedbackImagemPath;

            await _revisaoRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _revisaoRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Revisao com ID {id} nao encontrada.");

            await _revisaoRepository.DeleteAsync(id);
        }
    }
}
