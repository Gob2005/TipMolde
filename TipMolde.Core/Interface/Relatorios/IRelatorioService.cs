namespace TipMolde.Core.Interface.Relatorios
{
    public interface IRelatorioService
    {
        Task<(byte[] Content, string FileName)> GerarCicloVidaMoldePdfAsync(int moldeId);
        //Task<(byte[] Content, string FileName)> GerarFichaPdfFLTAsync(int fichaId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFLTAsync(int fichaId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFREAsync(int fichaId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFRMAsync(int fichaId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFRAAsync(int fichaId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFOPAsync(int fichaId);

    }
}
