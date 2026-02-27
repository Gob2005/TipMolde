using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Models;

namespace TipMolde.Infrastutura.Service
{
    public class MoldeService : IMoldeService
    {
        private readonly IMoldeRepository _moldeRepository;

        public MoldeService(IMoldeRepository moldeRepository)
        {
            _moldeRepository = moldeRepository;
        }

        public async Task<IEnumerable<Molde>> GetAllMoldesAsync()
        {
            return await _moldeRepository.GetAllAsync();
        }

        public async Task<Molde> GetMoldeByIdAsync(int id)
        {
            return await _moldeRepository.GetByIdAsync(id);
        }

        public async Task<Molde> CreateMoldeAsync(Molde molde)
        {
            var cliente = await _moldeRepository.GetClienteByIdAsync(molde.Cliente.Cliente_id);
            if (cliente == null) throw new Exception($"Cliente com ID {molde.Cliente.Cliente_id} nao encontrado.");
            if (string.IsNullOrEmpty(molde.Dimensoes_molde)) throw new Exception("Dimensoes do molde e obrigatorio.");
            if (molde.Peso_estimado <= 0) throw new Exception("Peso estimado deve ser maior que zero.");
            if (molde.Numero_cavidades <= 0) throw new Exception("Numero de cavidades deve ser maior que zero.");
            await _moldeRepository.AddAsync(molde);
            return molde;
        }

        public async Task UpdateMoldeAsync(Molde molde)
        {
            await _moldeRepository.UpdateAsync(molde);
        }

        public async Task DeleteMoldeAsync(int id)
        {
            var user = await _moldeRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception($"User com ID {id} nao encontrado.");
            }
            await _moldeRepository.DeleteAsync(id);
        }

        public async Task<Cliente> GetClienteByIdAsync(int id)
        {
            return await _moldeRepository.GetClienteByIdAsync(id);
        }
    }
}
