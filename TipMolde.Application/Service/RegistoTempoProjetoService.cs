using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Service
{
    public class RegistoTempoProjetoService : IRegistoTempoProjetoService
    {
        private readonly IRegistoTempoProjetoRepository _registoRepository;
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUserRepository _userRepository;

        public RegistoTempoProjetoService(
            IRegistoTempoProjetoRepository registoRepository,
            IProjetoRepository projetoRepository,
            IUserRepository userRepository)
        {
            _registoRepository = registoRepository;
            _projetoRepository = projetoRepository;
            _userRepository = userRepository;
        }

        public Task<IEnumerable<RegistoTempoProjeto>> GetHistoricoAsync(int projetoId, int autorId) =>
            _registoRepository.GetHistoricoAsync(projetoId, autorId);

        public async Task<RegistoTempoProjeto> CreateRegistoAsync(RegistoTempoProjeto registo)
        {
            if (await _projetoRepository.GetByIdAsync(registo.Projeto_id) == null)
                throw new KeyNotFoundException("Projeto nao encontrado.");
            if (await _userRepository.GetByIdAsync(registo.Autor_id) == null)
                throw new KeyNotFoundException("Autor nao encontrado.");

            var ultimo = await _registoRepository.GetUltimoRegistoAsync(registo.Projeto_id, registo.Autor_id);
            ValidarTransicao(ultimo?.Estado_tempo, registo.Estado_tempo);

            registo.Data_hora = DateTime.UtcNow;
            await _registoRepository.AddAsync(registo);
            return registo;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _registoRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Registo de tempo com ID {id} nao encontrado.");

            await _registoRepository.DeleteAsync(id);
        }

        private static void ValidarTransicao(EstadoTempoProjeto? atual, EstadoTempoProjeto novo)
        {
            if (atual is null && novo != EstadoTempoProjeto.INICIADO)
                throw new ArgumentException("Primeiro estado deve ser INICIADO.");

            if (atual == EstadoTempoProjeto.INICIADO && novo is not (EstadoTempoProjeto.PAUSADO or EstadoTempoProjeto.CONCLUIDO))
                throw new ArgumentException("Transicao invalida.");

            if (atual == EstadoTempoProjeto.PAUSADO && novo != EstadoTempoProjeto.RETOMADO)
                throw new ArgumentException("Transicao invalida.");

            if (atual == EstadoTempoProjeto.RETOMADO && novo is not (EstadoTempoProjeto.PAUSADO or EstadoTempoProjeto.CONCLUIDO))
                throw new ArgumentException("Transicao invalida.");

            if (atual == EstadoTempoProjeto.CONCLUIDO)
                throw new ArgumentException("Projeto ja concluido.");
        }
    }
}
