using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Models;

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

        public async Task<Peca> CreatePecaAsync(Peca peca)
        {
            var molde = await _moldeRepository.GetByIdAsync(peca.Molde_id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {peca.Molde_id} nao encontrado.");

            var existing = await _pecaRepository.GetByNumberAsync(peca.Numero_peca, peca.Molde_id);
            if (existing is not null) throw new ArgumentException("Ja existe uma peca com este numero.");

            await _pecaRepository.AddAsync(peca);
            return peca;
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

        public async Task UpdatePecaAsync(Peca peca)
        {
            var existing = await _pecaRepository.GetByIdAsync(peca.Peca_id);
            if (existing == null)
                throw new KeyNotFoundException($"Peca com ID {peca.Peca_id} nao encontrado.");

            existing.Numero_peca = peca.Numero_peca > 0 ? peca.Numero_peca : existing.Numero_peca;
            existing.Prioridade = peca.Prioridade > 0 ? peca.Prioridade : existing.Prioridade;
            existing.Descricao = !string.IsNullOrWhiteSpace(peca.Descricao) ? peca.Descricao : existing.Descricao;

            await _pecaRepository.UpdateAsync(existing);
        }

        public Task<IEnumerable<Peca>> GetAllPecasAsync()
        {
            return _pecaRepository.GetAllAsync();
        }

        public Task<Peca?> GetPecaByIdAsync(int id)
        {
            return _pecaRepository.GetByIdAsync(id);
        }

        public Task<Peca?> GetPecaByNumberAsync(int peca_id, int molde_id)
        {
            return _pecaRepository.GetByNumberAsync(peca_id, molde_id);
        }
    }
}
