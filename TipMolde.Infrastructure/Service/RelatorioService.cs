using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TipMolde.Core.Interface.Relatorios;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Service
{
    public class RelatorioService : IRelatorioService
    {
        private readonly ApplicationDbContext _context;
        public RelatorioService(ApplicationDbContext context) => _context = context;

        public async Task<(byte[] Content, string FileName)> GerarCicloVidaMoldePdfAsync(int moldeId)
        {
            var molde = await _context.Moldes
                .Include(m => m.Especificacoes)
                .FirstOrDefaultAsync(m => m.Molde_id == moldeId)
                ?? throw new KeyNotFoundException("Molde nao encontrado.");

            QuestPDF.Settings.License = LicenseType.Community;
            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Relatorio Ciclo de Vida - Molde {molde.Numero}").Bold().FontSize(16);
                        col.Item().Text($"Nome: {molde.Nome}");
                        col.Item().Text($"Descricao: {molde.Descricao}");
                        col.Item().Text($"Cavidades: {molde.Numero_cavidades}");
                    });
                });
            }).GeneratePdf();

            return (bytes, $"ciclo_vida_molde_{moldeId}.pdf");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaPdfFTLAsync(int fichaId)
        {
            var ficha = await _context.FichasProducao
                .Include(f => f.EncomendaMolde).ThenInclude(em => em.Molde)
                .FirstOrDefaultAsync(f => f.FichaProducao_id == fichaId)
                ?? throw new KeyNotFoundException("Ficha nao encontrada.");

            QuestPDF.Settings.License = LicenseType.Community;
            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Ficha {ficha.Tipo}").Bold().FontSize(16);
                        col.Item().Text($"Data: {ficha.DataGeracao:yyyy-MM-dd HH:mm}");
                        col.Item().Text($"Molde: {ficha.EncomendaMolde?.Molde?.Numero}");
                    });
                });
            }).GeneratePdf();

            return (bytes, $"ficha_{fichaId}.pdf");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFTLAsync(int fichaId)
        {
            var ficha = await _context.FichasProducao
                .Include(f => f.EncomendaMolde).ThenInclude(em => em.Molde)
                .FirstOrDefaultAsync(f => f.FichaProducao_id == fichaId)
                ?? throw new KeyNotFoundException("Ficha nao encontrada.");

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Ficha");
            ws.Cell("A1").Value = "Tipo";
            ws.Cell("B1").Value = ficha.Tipo.ToString();
            ws.Cell("A2").Value = "Data";
            ws.Cell("B2").Value = ficha.DataGeracao;
            ws.Cell("A3").Value = "Molde";
            ws.Cell("B3").Value = ficha.EncomendaMolde?.Molde?.Numero;

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"ficha_{fichaId}.xlsx");
        }
    }
}
