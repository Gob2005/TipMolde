using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Infrastructure.Service
{
    public class FasesProducaoService : IFasesProducaoService
    {
        private readonly IFasesProducaoRepository _fpRepository;

        public FasesProducaoService(IFasesProducaoRepository fpRepository)
        {
            _fpRepository = fpRepository;
        }

        public Task<PagedResult<FasesProducao>> GetAllAsync(int page = 1, int pageSize = 10) =>
            _fpRepository.GetAllAsync(page, pageSize);

        public Task<FasesProducao?> GetByIdAsync(int id) => _fpRepository.GetByIdAsync(id);

        public async Task<FasesProducao> CreateAsync(FasesProducao fp)
        {
            var existing = await _fpRepository.GetByNomeAsync(fp.Nome);
            if (existing != null) throw new ArgumentException("Ja existe uma fase de producao com este nome.");

            await _fpRepository.AddAsync(fp);
            return fp;
        }

        public async Task UpdateAsync(FasesProducao fp)
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

        public async Task DeleteAsync(int id)
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
