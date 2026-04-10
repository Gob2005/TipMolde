namespace TipMolde.Application.Interface.Relatorios
{
    public interface IRelatorioService
    {
        Task<(byte[] Content, string FileName)> GerarCicloVidaMoldePdfAsync(int moldeId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFLTAsync(int fichaId, int userId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFREAsync(int fichaId, int userId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFRMAsync(int fichaId, int userId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFRAAsync(int fichaId, int userId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFOPAsync(int fichaId, int userId);
    }
}
