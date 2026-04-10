using Microsoft.AspNetCore.Http;
using TipMolde.Domain.Entities.Fichas;

namespace TipMolde.Application.Interface.Fichas.IFichaDocumento
{
    public interface IFichaDocumentoService
    {
        Task<FichaDocumento> GuardarGeradoAsync(int fichaId, byte[] content, string fileName, string tipoFicheiro, int userId, string origem);
        Task<FichaDocumento> UploadAsync(int fichaId, IFormFile file, int userId);
        Task<IEnumerable<FichaDocumento>> ListarAsync(int fichaId);
        Task<(byte[] Content, string FileName, string TipoFicheiro)> DownloadAsync(int documentoId);
    }
}
