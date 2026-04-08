namespace TipMolde.Core.Interface.Relatorios
{
    public interface IRelatorioService
    {
        Task<(byte[] Content, string FileName)> GerarCicloVidaMoldePdfAsync(int moldeId);
        Task<(byte[] Content, string FileName)> GerarFichaPdfFTLAsync(int fichaId);
        Task<(byte[] Content, string FileName)> GerarFichaExcelFTLAsync(int fichaId);
    }
}
