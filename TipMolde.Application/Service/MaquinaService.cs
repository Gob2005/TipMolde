using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Application.Interface.Producao.IMaquina;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Infrastructure.Service
{
    public class MaquinaService : IMaquinaService
    {
        private readonly IMaquinaRepository _maquinaRepository;

        public MaquinaService(IMaquinaRepository maquinaRepository)
        {
            _maquinaRepository = maquinaRepository;
        }

        public Task<PagedResult<Maquina>> GetAllAsync(int page = 1, int pageSize = 10) =>
            _maquinaRepository.GetAllAsync(page, pageSize);

        public Task<Maquina?> GetByIdAsync(int id) => _maquinaRepository.GetByIdUnicoAsync(id);

        public Task<IEnumerable<Maquina>> GetByEstadoAsync(EstadoMaquina estado) =>
            _maquinaRepository.GetByEstadoAsync(estado);

        public async Task<Maquina> CreateAsync(Maquina maquina)
        {
            if (string.IsNullOrWhiteSpace(maquina.NomeModelo))
                throw new ArgumentException("Nome/modelo e obrigatorio.");

            var existing = await _maquinaRepository.GetByIdUnicoAsync(maquina.Maquina_id);
            if (existing != null)
                throw new ArgumentException("Ja existe maquina com este id.");

            await _maquinaRepository.AddAsync(maquina);
            return maquina;
        }

        public async Task UpdateAsync(Maquina maquina)
        {
            var existing = await _maquinaRepository.GetByIdUnicoAsync(maquina.Maquina_id);
            if (existing == null)
                throw new KeyNotFoundException($"Maquina {maquina.Maquina_id} nao encontrada.");

            existing.NomeModelo = string.IsNullOrWhiteSpace(maquina.NomeModelo) ? existing.NomeModelo : maquina.NomeModelo.Trim();
            existing.IpAddress = string.IsNullOrWhiteSpace(maquina.IpAddress) ? existing.IpAddress : maquina.IpAddress.Trim();
            existing.Estado = maquina.Estado;

            await _maquinaRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _maquinaRepository.GetByIdUnicoAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Maquina {id} nao encontrada.");

            await _maquinaRepository.DeleteAsync(id);
        }
    }
}
