using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Infrastructure.Service
{
    public class ProjetoService : IProjetoService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IMoldeRepository _moldeRepository;

        public ProjetoService(IProjetoRepository projetoRepository, IMoldeRepository moldeRepository)
        {
            _projetoRepository = projetoRepository;
            _moldeRepository = moldeRepository;
        }

        public Task<PagedResult<Projeto>> GetAllAsync(int page = 1, int pageSize = 10) =>
            _projetoRepository.GetAllAsync(page, pageSize);

        public Task<Projeto?> GetByIdAsync(int id) => _projetoRepository.GetByIdAsync(id);

        public Task<Projeto?> GetWithRevisoesAsync(int id) => _projetoRepository.GetWithRevisoesAsync(id);

        public Task<IEnumerable<Projeto>> GetByMoldeIdAsync(int moldeId) => _projetoRepository.GetByMoldeIdAsync(moldeId);

        public async Task<Projeto> CreateAsync(Projeto projeto)
        {
            if (string.IsNullOrWhiteSpace(projeto.NomeProjeto))
                throw new ArgumentException("Nome do projeto e obrigatorio.");
            if (string.IsNullOrWhiteSpace(projeto.SoftwareUtilizado))
                throw new ArgumentException("Software utilizado e obrigatorio.");

            var molde = await _moldeRepository.GetByIdAsync(projeto.Molde_id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {projeto.Molde_id} nao encontrado.");

            await _projetoRepository.AddAsync(projeto);
            return projeto;
        }

        public async Task UpdateAsync(Projeto projeto)
        {
            var existing = await _projetoRepository.GetByIdAsync(projeto.Projeto_id);
            if (existing == null)
                throw new KeyNotFoundException($"Projeto com ID {projeto.Projeto_id} nao encontrado.");

            existing.NomeProjeto = string.IsNullOrWhiteSpace(projeto.NomeProjeto) ? existing.NomeProjeto : projeto.NomeProjeto.Trim();
            existing.SoftwareUtilizado = string.IsNullOrWhiteSpace(projeto.SoftwareUtilizado) ? existing.SoftwareUtilizado : projeto.SoftwareUtilizado.Trim();
            existing.TipoProjeto = projeto.TipoProjeto;

            await _projetoRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _projetoRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Projeto com ID {id} nao encontrado.");

            await _projetoRepository.DeleteAsync(id);
        }
    }
}
