using TipMolde.Core.Interface.Producao.IMolde;
using TipMolde.Core.Interface.Producao.IPeca;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Infrastructure.Service
{
    public class PecaService : IPecaService
    {
        private readonly IPecaRepository _pecaRepository;
        private readonly IMoldeRepository _moldeRepository;

        public PecaService(IPecaRepository pecaRepository, IMoldeRepository moldeRepository)
        {
            _pecaRepository = pecaRepository;
            _moldeRepository = moldeRepository;
        }

        public Task<IEnumerable<Peca>> GetAllPecasAsync() =>
            _pecaRepository.GetAllAsync();

        public Task<Peca?> GetPecaByIdAsync(int id) =>
            _pecaRepository.GetByIdAsync(id);

        public Task<IEnumerable<Peca>> GetByMoldeIdAsync(int moldeId) =>
            _pecaRepository.GetByMoldeIdAsync(moldeId);

        public Task<Peca?> GetByDesignacaoAsync(string designacao, int moldeId) =>
            _pecaRepository.GetByDesignacaoAsync(designacao, moldeId);

        public async Task<Peca> CreatePecaAsync(Peca peca)
        {
            var molde = await _moldeRepository.GetByIdAsync(peca.Molde_id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {peca.Molde_id} nao encontrado.");

            if (string.IsNullOrWhiteSpace(peca.Designacao))
                throw new ArgumentException("Designacao e obrigatoria.");

            var existing = await _pecaRepository.GetByDesignacaoAsync(peca.Designacao, peca.Molde_id);
            if (existing is not null) throw new ArgumentException("Ja existe uma peca com esta designacao.");

            await _pecaRepository.AddAsync(peca);
            return peca;
        }
        public async Task UpdatePecaAsync(Peca peca)
        {
            var existing = await _pecaRepository.GetByIdAsync(peca.Peca_id);
            if (existing == null)
                throw new KeyNotFoundException($"Peca com ID {peca.Peca_id} nao encontrado.");

            existing.Designacao = string.IsNullOrWhiteSpace(peca.Designacao) ? existing.Designacao : peca.Designacao.Trim();
            existing.Prioridade = peca.Prioridade > 0 ? peca.Prioridade : existing.Prioridade;
            existing.MaterialDesignacao = peca.MaterialDesignacao ?? existing.MaterialDesignacao;
            existing.MaterialRecebido = peca.MaterialRecebido;

            await _pecaRepository.UpdateAsync(existing);
        }

        public async Task DeletePecaAsync(int id)
        {
            var peca = await _pecaRepository.GetByIdAsync(id);
            if (peca == null)
            {
                throw new KeyNotFoundException($"Peca com ID {id} nao encontrado.");
            }

            await _pecaRepository.DeleteAsync(id);
        }
    }
}
