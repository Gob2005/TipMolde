using TipMolde.Application.Interface.Comercio.IEncomendaMolde;
using TipMolde.Application.Interface.Fichas.IFichaProducao;
using TipMolde.Domain.Entities.Fichas;

namespace TipMolde.Application.Service
{
    public class FichaProducaoService : IFichaProducaoService
    {
        private readonly IFichaProducaoRepository _fichaRepository;
        private readonly IEncomendaMoldeRepository _encomendaMoldeRepository;

        public FichaProducaoService(
            IFichaProducaoRepository fichaRepository,
            IEncomendaMoldeRepository encomendaMoldeRepository)
        {
            _fichaRepository = fichaRepository;
            _encomendaMoldeRepository = encomendaMoldeRepository;
        }

        public Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId) =>
            _fichaRepository.GetByEncomendaMoldeIdAsync(encomendaMoldeId);

        public Task<FichaProducao?> GetByIdWithHeaderAsync(int id) =>
            _fichaRepository.GetByIdWithHeaderAsync(id);

        public Task<FichaProducao?> GetFLTByIdAsync(int id) =>
            _fichaRepository.GetFLTByIdAsync(id);

        public async Task<FichaProducao> CreateAsync(FichaProducao ficha)
        {
            var link = await _encomendaMoldeRepository.GetByIdAsync(ficha.EncomendaMolde_id);
            if (link == null)
                throw new KeyNotFoundException($"EncomendaMolde com ID {ficha.EncomendaMolde_id} nao encontrado.");

            ficha.DataCriacao = DateTime.UtcNow;
            await _fichaRepository.AddAsync(ficha);
            return ficha;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _fichaRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"FichaProducao com ID {id} nao encontrada.");

            await _fichaRepository.DeleteAsync(id);
        }
    }
}
