using TipMolde.Core.Models.Fichas;

namespace TipMolde.Core.Interface.Fichas.IFichaDocumento
{
    public interface IFichaDocumentoRepository
    {
        Task<bool> FichaExisteAsync(int fichaId);
        Task<int> GetProximaVersaoAsync(int fichaId);
        Task DesativarVersoesAtivasAsync(int fichaId);
        Task AddAsync(FichaDocumento doc);
        Task<FichaDocumento?> GetByIdAsync(int id);
        Task<FichaDocumento?> GetAtivoByFichaIdAsync(int fichaId);
        Task<IEnumerable<FichaDocumento>> GetByFichaIdAsync(int fichaId);
    }
}
