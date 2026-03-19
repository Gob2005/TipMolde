using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Models;

namespace TipMolde.Infrastructure.Service
{
    public class MoldeService : IMoldeService
    {
        private readonly IMoldeRepository _moldeRepository;

        public MoldeService(IMoldeRepository moldeRepository)
        {
            _moldeRepository = moldeRepository;
        }

        public Task<IEnumerable<Molde>> GetAllMoldesAsync()
        {
            return _moldeRepository.GetAllAsync();
        }

        public Task<Molde?> GetMoldeByIdAsync(int id)
        {
            return _moldeRepository.GetByIdAsync(id);
        }

        public async Task<Molde> CreateMoldeAsync(Molde molde)
        {
            if (string.IsNullOrWhiteSpace(molde.Dimensoes_molde)) throw new ArgumentException("Dimensoes do molde sao obrigatorias.");
            if (molde.Peso_estimado <= 0) throw new ArgumentException("Peso estimado deve ser maior que zero.");
            if (molde.Numero_cavidades <= 0) throw new ArgumentException("Numero de cavidades deve ser maior que zero.");

            await _moldeRepository.AddAsync(molde);
            return molde;
        }

        public Task UpdateMoldeAsync(Molde molde)
        {
            return _moldeRepository.UpdateAsync(molde);
        }

        public async Task DeleteMoldeAsync(int id)
        {
            var molde = await _moldeRepository.GetByIdAsync(id);
            if (molde == null)
            {
                throw new KeyNotFoundException($"Molde com ID {id} nao encontrado.");
            }

            await _moldeRepository.DeleteAsync(id);
        }

        public Task<Cliente?> GetClienteByIdAsync(int id)
        {
            return _moldeRepository.GetClienteByIdAsync(id);
        }
    }
}
