using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Microsoft.Extensions.Configuration;
using TipMolde.Core.Interface.Relatorios;

namespace TipMolde.Infrastructure.Service
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly IConfiguration _configuration;

        public RelatorioService(IRelatorioRepository relatorioRepository, IConfiguration configuration)
        {
            _relatorioRepository = relatorioRepository;
            _configuration = configuration;
        }

        public async Task<(byte[] Content, string FileName)> GerarCicloVidaMoldePdfAsync(int moldeId)
        {
            var molde = await _relatorioRepository.GetMoldeComEspecificacoesAsync(moldeId)
                ?? throw new KeyNotFoundException($"Molde {moldeId} nao encontrado.");

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
            var ficha = await _relatorioRepository.GetFichaFltCompletaAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

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
            var ficha = await _relatorioRepository.GetFichaFltCompletaAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

            var em = ficha.EncomendaMolde ?? throw new InvalidOperationException("Ficha sem EncomendaMolde.");
            var en = em.Encomenda ?? throw new InvalidOperationException("Encomenda em falta.");
            var cl = en.Cliente ?? throw new InvalidOperationException("Cliente em falta.");
            var mo = em.Molde ?? throw new InvalidOperationException("Molde em falta.");
            var sp = mo.Especificacoes;

            var templatePath = _configuration["Templates:FichasPath"]
                ?? throw new InvalidOperationException("Templates:FichasPath não configurado.");

            var folhaFlt = _configuration["Templates:FolhaFLT"] ?? "FLT - TM.04.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFlt);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFlt}' não encontrada no template.");

            ws.Cell("I4").Value = DateTime.Now.ToString("dd/MM/yyyy");
            ws.Cell("C6").Value = mo.Nome;
            ws.Cell("J6").Value = mo.Numero;
            ws.Cell("D43").Value = cl.Nome;
            ws.Cell("D44").Value = en.NomeServicoCliente;
            ws.Cell("E45").Value = en.NumeroProjetoCliente;
            ws.Cell("I45").Value = mo.NumeroMoldeCliente;
            ws.Cell("E46").Value = en.NomeResponsavelCliente;
            ws.Cell("B48").Value = em.DataEntregaPrevista.ToString("dd/MM/yyyy");

            ws.Cell("B9").Value = mo?.ImagemCapaPath;
            ws.Cell("D28").Value = mo.Numero_cavidades;
            ws.Cell("G28").Value = sp?.MaterialInjecao;
            ws.Cell("J28").Value = sp?.Contracao;
            ws.Cell("E29").Value = sp?.TipoInjecao;
            ws.Cell("J29").Value = sp?.AcabamentoPeca;
            ws.Cell("D30").Value = sp?.MaterialMacho;
            ws.Cell("D31").Value = sp?.MaterialCavidade;
            ws.Cell("D32").Value = sp?.MaterialMovimentos;
            ws.Cell("E34").Value = sp?.SistemaInjecao;

            ws.Cell("E38").Value = mo.TipoPedido.ToString();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"ficha_FTL_{fichaId}.xlsx");
        }
    }
}
