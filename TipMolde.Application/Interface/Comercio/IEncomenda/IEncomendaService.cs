using TipMolde.Application.DTOs.EncomendaDTO;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Interface.Comercio.IEncomenda
{
    /// <summary>
    /// Define os casos de uso da feature Encomenda.
    /// </summary>
    public interface IEncomendaService
    {
        /// <summary>Lista encomendas com paginacao.</summary>
        Task<PagedResult<ResponseEncomendaDTO>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>Obtem encomenda por ID.</summary>
        Task<ResponseEncomendaDTO?> GetByIdAsync(int id);

        /// <summary>Obtem encomenda com moldes associados.</summary>
        Task<ResponseEncomendaDTO?> GetEncomendaWithMoldesAsync(int id);

        /// <summary>Lista encomendas por estado.</summary>
        Task<PagedResult<ResponseEncomendaDTO>> GetByEstadoAsync(EstadoEncomenda estado, int page = 1, int pageSize = 10);

        /// <summary>Lista encomendas por concluir.</summary>
        Task<PagedResult<ResponseEncomendaDTO>> GetEncomendasPorConcluirAsync(int page = 1, int pageSize = 10);

        /// <summary>Obtem encomenda por numero do cliente.</summary>
        Task<ResponseEncomendaDTO?> GetByNumeroEncomendaClienteAsync(string numero);

        /// <summary>Cria encomenda com base em DTO.</summary>
        Task<ResponseEncomendaDTO> CreateAsync(CreateEncomendaDTO dto);

        /// <summary>Atualiza parcialmente uma encomenda por ID.</summary>
        Task UpdateAsync(int id, UpdateEncomendaDTO dto);

        /// <summary>Atualiza estado da encomenda.</summary>
        Task UpdateEstadoAsync(int id, UpdateEstadoEncomendaDTO dto);

        /// <summary>Elimina encomenda por ID.</summary>
        Task DeleteAsync(int id);
    }
}
