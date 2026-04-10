using TipMolde.Application.Interface.Comercio.IEncomenda;
using TipMolde.Application.Interface.Comercio.IEncomendaMolde;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Infrastructure.Service
{
    public class EncomendaMoldeService : IEncomendaMoldeService
    {
        private readonly IEncomendaMoldeRepository _repo;
        private readonly IEncomendaRepository _encomendaRepo;
        private readonly IMoldeRepository _moldeRepo;

        public EncomendaMoldeService(
            IEncomendaMoldeRepository repo,
            IEncomendaRepository encomendaRepo,
            IMoldeRepository moldeRepo)
        {
            _repo = repo;
            _encomendaRepo = encomendaRepo;
            _moldeRepo = moldeRepo;
        }

        public Task<IEnumerable<EncomendaMolde>> GetByEncomendaIdAsync(int encomendaId) =>
            _repo.GetByEncomendaIdAsync(encomendaId);

        public Task<IEnumerable<EncomendaMolde>> GetByMoldeIdAsync(int moldeId) =>
            _repo.GetByMoldeIdAsync(moldeId);

        public async Task<EncomendaMolde> CreateAsync(EncomendaMolde link)
        {
            var encomenda = await _encomendaRepo.GetByIdAsync(link.Encomenda_id);
            if (encomenda == null)
                throw new KeyNotFoundException($"Encomenda com ID {link.Encomenda_id} nao encontrada.");

            var molde = await _moldeRepo.GetByIdAsync(link.Molde_id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {link.Molde_id} nao encontrado.");

            await _repo.AddAsync(link);
            return link;
        }

        public async Task UpdateAsync(EncomendaMolde link)
        {
            var existente = await _repo.GetByIdAsync(link.EncomendaMolde_id);
            if (existente == null)
                throw new KeyNotFoundException($"EncomendaMolde com ID {link.EncomendaMolde_id} nao encontrada.");

            existente.Quantidade = link.Quantidade > 0 ? link.Quantidade : existente.Quantidade;
            existente.Prioridade = link.Prioridade > 0 ? link.Prioridade : existente.Prioridade;
            existente.DataEntregaPrevista = link.DataEntregaPrevista != default ? link.DataEntregaPrevista : existente.DataEntregaPrevista;

            await _repo.UpdateAsync(existente);
        }

        public async Task DeleteAsync(int id)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"EncomendaMolde com ID {id} nao encontrada.");

            await _repo.DeleteAsync(id);
        }
    }
}
