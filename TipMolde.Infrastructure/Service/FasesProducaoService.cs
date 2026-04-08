using TipMolde.Core.Interface.Producao.IFasesProducao;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Infrastructure.Service
{
    public class FasesProducaoService : IFasesProducaoService
    {
        private readonly IFasesProducaoRepository _fpRepository;

        public FasesProducaoService(IFasesProducaoRepository fpRepository)
        {
            _fpRepository = fpRepository;
        }

        public Task<IEnumerable<FasesProducao>> GetAllFases_producaoAsync() => _fpRepository.GetAllAsync();

        public Task<FasesProducao?> GetFase_producaoByIdAsync(int id) => _fpRepository.GetByIdAsync(id);

        public async Task<FasesProducao> CreateFase_producaoAsync(FasesProducao fp)
        {
            var existing = await _fpRepository.GetByNomeAsync(fp.Nome);
            if (existing != null) throw new ArgumentException("Ja existe uma fase de producao com este nome.");

            await _fpRepository.AddAsync(fp);
            return fp;
        }

        public async Task UpdateFase_producaoAsync(FasesProducao fp)
        {
            var existing = await _fpRepository.GetByIdAsync(fp.Fases_producao_id);
            if (existing == null)
                throw new KeyNotFoundException($"Fase de Produção com ID {fp.Fases_producao_id} não encontrada.");

            if (fp.Nome != existing.Nome)
            {
                var byNome = await _fpRepository.GetByNomeAsync(fp.Nome);
                if (byNome != null && byNome.Fases_producao_id != existing.Fases_producao_id)
                    throw new ArgumentException("Já existe uma fase de produção com este nome.");

                existing.Nome = fp.Nome;
            }

            existing.Descricao = string.IsNullOrWhiteSpace(fp.Descricao) ? existing.Descricao : fp.Descricao.Trim();

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
    }
}
