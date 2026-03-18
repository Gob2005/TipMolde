using TipMolde.Core.Enums;
using TipMolde.Core.Interface.IFases_producao;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Interface.IRegistosProducao;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;

namespace TipMolde.Infrastructure.Service
{
    public class RegistosProducaoService : IRegistosProducaoService
    {
        private readonly IRegistosProducaoRepository _rpRepository;
        private readonly IMoldeRepository _moldeRepository;
        private readonly IFasesProducaoRepository _fpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPecaRepository _pecaRepository;

        public RegistosProducaoService(IRegistosProducaoRepository rpRepository)
        {
            _rpRepository = rpRepository;
        }

        public async Task<RegistosProducao> CreateRegistoProducaoAsync(RegistosProducao registo)
        {
            var molde = await _moldeRepository.GetByIdAsync(registo.Molde_id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {registo.Molde_id} nao encontrado.");

            var fase = await _fpRepository.GetByIdAsync(registo.Fase_id);
            if (fase == null)
                throw new KeyNotFoundException($"Fase com ID {registo.Fase_id} nao encontrada.");

            var operador = await _userRepository.GetByIdAsync(registo.Operador_id);
            if (operador == null)
                throw new KeyNotFoundException($"Operador com ID {registo.Operador_id} nao encontrado.");

            var peca = await _pecaRepository.GetByIdAsync(registo.Peca_id);
            if (peca == null)
                throw new KeyNotFoundException($"Peca com ID {registo.Peca_id} nao encontrada.");

            var ultimoRegisto = await _rpRepository.GetUltimoRegistoAsync(
                registo.Molde_id, registo.Fase_id, registo.Peca_id);

            var estadoActual = ultimoRegisto?.Estado_producao ?? EstadoProducao.PREPARACAO;
            ValidarTransicaoEstado(estadoActual, registo.Estado_producao);

            registo.Data_hora = DateTime.UtcNow;

            await _rpRepository.AddAsync(registo);
            return registo;
        }

        public async Task DeleteRegistoProducaoAsync(int id)
        {
            var registo = await _rpRepository.GetByIdAsync(id);
            if (registo == null)
                throw new KeyNotFoundException($"Registo de Producao com ID {id} nao encontrado.");

            await _rpRepository.DeleteAsync(id);
        }

        public Task UpdateRegistoProducaoAsync(RegistosProducao registo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RegistosProducao>> GetAllRegistosProducaoAsync()
        {
            return _rpRepository.GetAllAsync();
        }

        public Task<RegistosProducao?> GetRegistoProducaoByIdAsync(int id)
        {
            return _rpRepository.GetByIdAsync(id);
        }

        public Task GetHistoricoAsync(int moldeId, int faseId, int pecaId, int operadorId)
        {
            throw new NotImplementedException();
        }
    }
}
