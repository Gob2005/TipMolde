using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Service
{
    public class MoldeService : IMoldeService
    {
        private readonly IMoldeRepository _moldeRepository;

        public MoldeService(IMoldeRepository moldeRepository)
        {
            _moldeRepository = moldeRepository;
        }

        public Task<PagedResult<Molde>> GetAllAsync(int page = 1, int pageSize = 10) =>
            _moldeRepository.GetAllAsync(page, pageSize);

        public Task<Molde?> GetByIdAsync(int id) =>
            _moldeRepository.GetByIdAsync(id);

        public Task<Molde?> GetByIdWithSpecsAsync(int id) =>
            _moldeRepository.GetByIdWithSpecsAsync(id);

        public Task<IEnumerable<Molde>> GetByEncomendaIdAsync(int encomendaId) =>
            _moldeRepository.GetByEncomendaIdAsync(encomendaId);

        public Task<Molde?> GetByNumeroAsync(string numero) =>
            _moldeRepository.GetByNumeroAsync(numero);

        public async Task<bool> ExistsByNumeroAsync(string numero)
        {
            var molde = await _moldeRepository.GetByNumeroAsync(numero);
            return molde is not null;
        }

        public async Task<Molde> CreateAsync(Molde molde, EspecificacoesTecnicas specs)
        {
            if (string.IsNullOrWhiteSpace(molde.Numero))
                throw new ArgumentException("Numero do molde e obrigatorio.");

            var existente = await _moldeRepository.GetByNumeroAsync(molde.Numero);
            if (existente != null)
                throw new ArgumentException("Ja existe um molde com este numero.");

            await _moldeRepository.AddMoldeWithSpecsAsync(molde, specs);
            return molde;
        }


        public async Task UpdateAsync(Molde molde, EspecificacoesTecnicas? specs)
        {
            var existente = await _moldeRepository.GetByIdWithSpecsAsync(molde.Molde_id);
            if (existente == null)
                throw new KeyNotFoundException($"Molde com ID {molde.Molde_id} nao encontrado.");

            existente.Numero = string.IsNullOrWhiteSpace(molde.Numero) ? existente.Numero : molde.Numero.Trim();
            existente.Nome = molde.Nome ?? existente.Nome;
            existente.ImagemCapaPath = string.IsNullOrWhiteSpace(molde.ImagemCapaPath)
                ? existente.ImagemCapaPath
                : molde.ImagemCapaPath.Trim();
            existente.Descricao = molde.Descricao ?? existente.Descricao;
            existente.Numero_cavidades = molde.Numero_cavidades > 0 ? molde.Numero_cavidades : existente.Numero_cavidades;
            existente.TipoPedido = molde.TipoPedido;

            if (specs != null)
            {
                existente.Especificacoes ??= new EspecificacoesTecnicas { Molde_id = existente.Molde_id };
                existente.Especificacoes.Largura = specs.Largura ?? existente.Especificacoes.Largura;
                existente.Especificacoes.Comprimento = specs.Comprimento ?? existente.Especificacoes.Comprimento;
                existente.Especificacoes.Altura = specs.Altura ?? existente.Especificacoes.Altura;
                existente.Especificacoes.PesoEstimado = specs.PesoEstimado ?? existente.Especificacoes.PesoEstimado;
                existente.Especificacoes.TipoInjecao = specs.TipoInjecao ?? existente.Especificacoes.TipoInjecao;
                existente.Especificacoes.SistemaInjecao = specs.SistemaInjecao ?? existente.Especificacoes.SistemaInjecao;
                existente.Especificacoes.Contracao = specs.Contracao ?? existente.Especificacoes.Contracao;
                existente.Especificacoes.AcabamentoPeca = specs.AcabamentoPeca ?? existente.Especificacoes.AcabamentoPeca;
                existente.Especificacoes.Cor = specs.Cor ?? existente.Especificacoes.Cor;
                existente.Especificacoes.MaterialMacho = specs.MaterialMacho ?? existente.Especificacoes.MaterialMacho;
                existente.Especificacoes.MaterialCavidade = specs.MaterialCavidade ?? existente.Especificacoes.MaterialCavidade;
                existente.Especificacoes.MaterialMovimentos = specs.MaterialMovimentos ?? existente.Especificacoes.MaterialMovimentos;
                existente.Especificacoes.MaterialInjecao = specs.MaterialInjecao ?? existente.Especificacoes.MaterialInjecao;
            }

            await _moldeRepository.UpdateAsync(existente);
        }

        public async Task DeleteAsync(int id)
        {
            var molde = await _moldeRepository.GetByIdAsync(id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {id} nao encontrado.");

            await _moldeRepository.DeleteAsync(id);
        }
    }
}
