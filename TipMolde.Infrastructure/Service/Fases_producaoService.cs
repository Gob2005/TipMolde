using TipMolde.Core.Interface.IFases_producao;
using TipMolde.Core.Models;

namespace TipMolde.Infrastructure.Service
{
    public class Fases_producaoService : IFases_producaoService
    {
        private readonly IFases_producaoRepository _fpRepository;

        public Fases_producaoService(IFases_producaoRepository fpRepository)
        {
            _fpRepository = fpRepository;
        }

        public async Task<Fases_producao> CreateFase_producaoAsync(Fases_producao fp)
        {
            var existing = await _fpRepository.GetByNomeAsync(fp.Nome);
            if (existing != null) throw new ArgumentException("Ja existe uma fase de producao com este nome.");

            await _fpRepository.AddAsync(fp);
            return fp;
        }

        public async Task UpdateFase_producaoAsync(Fases_producao fp)
        {
            var existing = await _fpRepository.GetByIdAsync(fp.Fases_producao_id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Fase de Producao com ID {fp.Fases_producao_id} nao encontrada.");
            }

            if (fp.Nome != existing.Nome)
            {
                var byNome = await _fpRepository.GetByNomeAsync(fp.Nome);
                if (byNome != null && byNome.Fases_producao_id != existing.Fases_producao_id)
                {
                    throw new ArgumentException("Ja existe uma fase de producao com este nome.");
                }
                existing.Nome = fp.Nome;
            }

            if (!string.IsNullOrWhiteSpace(fp.Descricao))
            {
                existing.Descricao = fp.Descricao.Trim();
            }

            await _fpRepository.UpdateAsync(existing);
        }

        public async Task DeleteFase_producaoAsync(int id)
        {
            var fp = await _fpRepository.GetByIdAsync(id);
            if (fp == null)
            {
                throw new KeyNotFoundException($"Fases de Producao com ID {id} nao encontrado.");
            }

            await _fpRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Fases_producao>> GetAllFases_producaoAsync()
        {
            return _fpRepository.GetAllAsync();
        }

        public Task<Fases_producao> GetFase_producaoByIdAsync(int id)
        {
            return _fpRepository.GetByIdAsync(id);
        }
    }
}
