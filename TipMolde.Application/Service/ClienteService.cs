using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso de negocio para gestao de clientes.
    /// </summary>
    /// <remarks>
    /// Aplica validacoes funcionais, regras de unicidade e delega operacoes de persistencia ao repositorio.
    /// </remarks>
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        /// <summary>
        /// Construtor de ClienteService.
        /// </summary>
        /// <param name="clienteRepository">Repositorio responsavel pelo acesso aos dados de cliente.</param>
        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        /// <summary>
        /// Lista clientes com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com clientes e metadados de navegacao.</returns>
        public Task<PagedResult<Cliente>> GetAllAsync(int page = 1, int pageSize = 10) =>
            _clienteRepository.GetAllAsync(page, pageSize);

        /// <summary>
        /// Obtem um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do cliente.</param>
        /// <returns>Cliente encontrado ou nulo quando nao existe registo.</returns>
        public Task<Cliente?> GetByIdAsync(int id)
        {
            return _clienteRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Pesquisa clientes por nome.
        /// </summary>
        /// <remarks>
        /// Quando o termo de pesquisa e vazio devolve colecao vazia para evitar consulta desnecessaria.
        /// </remarks>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome.</param>
        /// <returns>Colecao de clientes que correspondem ao termo informado.</returns>
        public Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Task.FromResult(Enumerable.Empty<Cliente>());
            return _clienteRepository.SearchByNameAsync(searchTerm);
        }

        /// <summary>
        /// Pesquisa clientes por sigla.
        /// </summary>
        /// <remarks>
        /// Quando o termo de pesquisa e vazio devolve colecao vazia para evitar consulta desnecessaria.
        /// </remarks>
        /// <param name="searchTerm">Termo parcial para pesquisa na sigla.</param>
        /// <returns>Colecao de clientes que correspondem ao termo informado.</returns>
        public Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Task.FromResult(Enumerable.Empty<Cliente>());
            return _clienteRepository.SearchBySiglaAsync(searchTerm);
        }

        /// <summary>
        /// Obtem um cliente com as encomendas associadas.
        /// </summary>
        /// <param name="clienteId">Identificador unico do cliente.</param>
        /// <returns>Cliente com encomendas ou nulo quando nao existe registo.</returns>
        public Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId)
        {
            return _clienteRepository.GetClienteWithEncomendasAsync(clienteId);
        }

        /// <summary>
        /// Cria um novo cliente apos validacoes de obrigatoriedade e unicidade.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Valida campos obrigatorios Nome, NIF e Sigla.
        /// 2. Garante unicidade de NIF e Sigla.
        /// 3. Normaliza campos textuais removendo espacos nas extremidades.
        /// 4. Persiste o cliente.
        /// </remarks>
        /// <param name="cliente">Entidade de cliente a validar e persistir.</param>
        /// <returns>Cliente criado apos validacao e persistencia.</returns>
        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                throw new ArgumentException("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(cliente.NIF))
                throw new ArgumentException("NIF e obrigatorio.");
            if (string.IsNullOrWhiteSpace(cliente.Sigla))
                throw new ArgumentException("Sigla e obrigatoria.");

            var nifExists = await _clienteRepository.GetByNifAsync(cliente.NIF.Trim());
            if (nifExists != null)
                throw new ArgumentException("Ja existe cliente com este NIF.");

            var siglaExists = await _clienteRepository.GetBySiglaAsync(cliente.Sigla.Trim());
            if (siglaExists != null)
                throw new ArgumentException("Ja existe cliente com esta Sigla.");

            cliente.Nome = cliente.Nome.Trim();
            cliente.NIF = cliente.NIF.Trim();
            cliente.Sigla = cliente.Sigla.Trim();

            await _clienteRepository.AddAsync(cliente);
            return cliente;
        }

        /// <summary>
        /// Atualiza dados de um cliente existente com validacao de unicidade.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Confirma existencia do cliente.
        /// 2. Valida conflito de NIF e Sigla quando houver alteracao desses campos.
        /// 3. Atualiza apenas campos informados, preservando os valores existentes quando em branco.
        /// 4. Persiste as alteracoes.
        /// </remarks>
        /// <param name="cliente">Entidade com identificador e dados a atualizar.</param>
        /// <returns>Task assincrona concluida apos atualizacao do cliente.</returns>
        public async Task UpdateAsync(Cliente cliente)
        {
            var existing = await _clienteRepository.GetByIdAsync(cliente.Cliente_id);
            if (existing == null)
                throw new KeyNotFoundException($"Cliente com ID {cliente.Cliente_id} não encontrado.");

            if (!string.IsNullOrWhiteSpace(cliente.NIF) && cliente.NIF != existing.NIF)
            {
                var nifExists = await _clienteRepository.GetByNifAsync(cliente.NIF.Trim());
                if (nifExists != null && nifExists.Cliente_id != existing.Cliente_id)
                    throw new ArgumentException("Ja existe cliente com este NIF.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Sigla) && cliente.Sigla != existing.Sigla)
            {
                var siglaExists = await _clienteRepository.GetBySiglaAsync(cliente.Sigla.Trim());
                if (siglaExists != null && siglaExists.Cliente_id != existing.Cliente_id)
                    throw new ArgumentException("Ja existe cliente com esta Sigla.");
            }

            existing.Nome = string.IsNullOrWhiteSpace(cliente.Nome) ? existing.Nome : cliente.Nome.Trim();
            existing.Pais = string.IsNullOrWhiteSpace(cliente.Pais) ? existing.Pais : cliente.Pais.Trim();
            existing.Email = string.IsNullOrWhiteSpace(cliente.Email) ? existing.Email : cliente.Email.Trim();
            existing.Telefone = string.IsNullOrWhiteSpace(cliente.Telefone) ? existing.Telefone : cliente.Telefone.Trim();
            existing.NIF = string.IsNullOrWhiteSpace(cliente.NIF) ? existing.NIF : cliente.NIF.Trim();
            existing.Sigla = string.IsNullOrWhiteSpace(cliente.Sigla) ? existing.Sigla : cliente.Sigla.Trim();

            await _clienteRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Remove um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do cliente a remover.</param>
        /// <returns>Task assincrona concluida apos remocao do cliente.</returns>
        public async Task DeleteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) throw new KeyNotFoundException($"Cliente com ID {id} nao encontrado.");
            await _clienteRepository.DeleteAsync(id);
        }
    }
}
