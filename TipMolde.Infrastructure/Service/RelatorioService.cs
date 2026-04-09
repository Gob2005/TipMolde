using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Fichas.IFichaDocumento;
using TipMolde.Core.Interface.Relatorios;
using TipMolde.Core.Models.Comercio;
using TipMolde.Core.Models.Fichas;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Infrastructure.Service
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly IConfiguration _configuration;
        private readonly IFichaDocumentoService _fichaDocumentoService;

        public RelatorioService(IRelatorioRepository relatorioRepository, IConfiguration configuration, IFichaDocumentoService fichaDocumentoService)
        {
            _relatorioRepository = relatorioRepository;
            _configuration = configuration;
            _fichaDocumentoService = fichaDocumentoService;
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

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFLTAsync(int fichaId, int userId)
        {
            var context = await GetFichaContextAsync(fichaId);

            var templatePath = _configuration["Templates:FichaFLT"]
                ?? throw new InvalidOperationException("Templates:FichaFLT não configurado.");

            var folhaFlt = _configuration["Templates:FolhaFLT"] ?? "FLT - TM.04.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFlt);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFlt}' não encontrada no template.");

            FillHeaderComum(ws, context);


            var imagePathRaw = context.Molde.ImagemCapaPath;

            var imagePath = TryAddImage(ws, imagePathRaw);

            var area = ws.Range("B9:J24");
            var pic = ws.AddPicture(imagePath)
                        .MoveTo(area.FirstCell(), 5, 5)
                        .WithSize(550, 320);

            ws.Cell("D28").Value = context.Molde.Numero_cavidades;
            ws.Cell("G28").Value = context.Specs?.MaterialInjecao;
            ws.Cell("J28").Value = context.Specs?.Contracao;
            ws.Cell("E29").Value = context.Specs?.TipoInjecao;
            ws.Cell("J29").Value = context.Specs?.AcabamentoPeca;
            ws.Cell("D30").Value = context.Specs?.MaterialMacho;
            ws.Cell("D31").Value = context.Specs?.MaterialCavidade;
            ws.Cell("D32").Value = context.Specs?.MaterialMovimentos;
            ws.Cell("E34").Value = context.Specs?.SistemaInjecao;

            var cor = context.Specs?.Cor;

            SetX(ws, "F26", cor == CorMolde.MONOCOLOR);
            SetX(ws, "H26", cor == CorMolde.BICOLOR);
            SetX(ws, "J26", cor == CorMolde.OUTRO);

            SetX(ws, "G33", context.Specs?.LadoFixo == true);
            SetX(ws, "J33", context.Specs?.LadoMovel == true);

            ws.Cell("E38").Value = context.Molde.TipoPedido.ToString();

            ws.Cell("D43").Value = context.Cliente.Nome;
            ws.Cell("D44").Value = context.Encomenda.NomeServicoCliente;
            ws.Cell("E45").Value = context.Encomenda.NumeroProjetoCliente;
            ws.Cell("I45").Value = context.Molde.NumeroMoldeCliente;
            ws.Cell("E46").Value = context.Encomenda.NomeResponsavelCliente;
            ws.Cell("B48").Value = context.EncomendaMolde.DataEntregaPrevista.ToString("dd/MM/yyyy");
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            await _fichaDocumentoService.GuardarGeradoAsync(
                    fichaId,
                    ms.ToArray(),
                    $"ficha_FLT_{fichaId}.xlsx",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    userId,
                    "SISTEMA");
            return (ms.ToArray(), $"ficha_FTL_{fichaId}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFREAsync(int fichaId, int userId)
        {
            var context = await GetFichaContextAsync(fichaId);

            var templatePath = _configuration["Templates:FichaFRE"]
                ?? throw new InvalidOperationException("Templates:FichaFRE não configurado.");

            var folhaFre = _configuration["Templates:FolhaFRE"] ?? "FRE - TM.08.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFre);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFre}' não encontrada no template.");

            FillHeaderComum(ws, context);

            var imagePathRaw = context.Molde.ImagemCapaPath;

            var imagePath = TryAddImage(ws, imagePathRaw);

            var area = ws.Range("B9:J17");
            var pic = ws.AddPicture(imagePath)
                        .MoveTo(area.FirstCell(), 5, 5)
                        .WithSize(550, 160);

            ws.Cell("D20").Value = context.Molde.Numero_cavidades;
            ws.Cell("G20").Value = context.Specs?.MaterialInjecao;
            ws.Cell("J20").Value = context.Specs?.Contracao;
            ws.Cell("E21").Value = context.Specs?.TipoInjecao;
            ws.Cell("J21").Value = context.Specs?.AcabamentoPeca;
            ws.Cell("E22").Value = context.Specs?.SistemaInjecao;

            var cor = context.Specs?.Cor;

            SetX(ws, "F18", cor == CorMolde.MONOCOLOR);
            SetX(ws, "H18", cor == CorMolde.BICOLOR);
            SetX(ws, "J18", cor == CorMolde.OUTRO);

            ws.Cell("D26").Value = context.Cliente.Nome;
            ws.Cell("D27").Value = context.Encomenda.NomeServicoCliente;
            ws.Cell("E28").Value = context.Encomenda.NumeroProjetoCliente;
            ws.Cell("I28").Value = context.Molde.NumeroMoldeCliente;
            ws.Cell("E29").Value = context.Encomenda.NomeResponsavelCliente;
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            await _fichaDocumentoService.GuardarGeradoAsync(
                    fichaId,
                    ms.ToArray(),
                    $"ficha_FRE_{fichaId}.xlsx",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    userId,
                    "SISTEMA");
            return (ms.ToArray(), $"ficha_FRE_{fichaId}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFRMAsync(int fichaId, int userId)
        {
            var context = await GetFichaContextAsync(fichaId);

            var templatePath = _configuration["Templates:FichaFRM"]
                ?? throw new InvalidOperationException("Templates:FichaFRM não configurado.");

            var folhaFrm = _configuration["Templates:FolhaFRM"] ?? "FRM - TM.09.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFrm);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFrm}' não encontrada no template.");

            FillHeaderComum(ws, context);

            FillBocoCliente(ws, context);

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            await _fichaDocumentoService.GuardarGeradoAsync(
                    fichaId,
                    ms.ToArray(),
                    $"ficha_FRE_{fichaId}.xlsx",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    userId,
                    "SISTEMA");
            return (ms.ToArray(), $"ficha_FRM_{fichaId}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFRAAsync(int fichaId, int userId)
        {
            var context = await GetFichaContextAsync(fichaId);

            var templatePath = _configuration["Templates:FichaFRA"]
                ?? throw new InvalidOperationException("Templates:FichaFRA não configurado.");

            var folhaFra = _configuration["Templates:FolhaFRA"] ?? "FRA - TM.010.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFra);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFra}' não encontrada no template.");

            FillHeaderComum(ws, context);

            FillBocoCliente(ws, context);

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            await _fichaDocumentoService.GuardarGeradoAsync(
                    fichaId,
                    ms.ToArray(),
                    $"ficha_FRA_{fichaId}.xlsx",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    userId,
                    "SISTEMA");
            return (ms.ToArray(), $"ficha_FRA_{fichaId}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFOPAsync(int fichaId, int userId)
        {
            var context = await GetFichaContextAsync(fichaId);

            var templatePath = _configuration["Templates:FichaFOP"]
                ?? throw new InvalidOperationException("Templates:FichaFOP não configurado.");

            var folhaFop = _configuration["Templates:FolhaFOP"] ?? "FOP - TM.07.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFop);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFop}' não encontrada no template.");

            FillHeaderComum(ws, context);

            FillBocoCliente(ws, context);

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
                await _fichaDocumentoService.GuardarGeradoAsync(
                        fichaId,
                        ms.ToArray(),
                        $"ficha_FOP_{fichaId}.xlsx",
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        userId,
                        "SISTEMA");
            return (ms.ToArray(), $"ficha_FOP_{fichaId}.xlsx");
        }

        private string TryAddImage(IXLWorksheet ws, string? imagePathRaw)
        {
            if (string.IsNullOrWhiteSpace(imagePathRaw))
                throw new ArgumentNullException("Imagem sem caminho");

            var imagePath = imagePathRaw;
            if (!Path.IsPathRooted(imagePath))
            {
                var uploadsRoot = _configuration["Uploads:RootPath"] ?? @"C:\Users\HP\Documents\TipMolde\Uploads";
                imagePath = Path.Combine(uploadsRoot, imagePath.TrimStart('\\', '/'));
            }

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Imagem não encontrada: {imagePath}");

            var ext = Path.GetExtension(imagePath).ToLowerInvariant();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                throw new InvalidOperationException("Formato de imagem não suportado.");
            return imagePath;
        }
        private async Task<FichaContext> GetFichaContextAsync(int fichaId)
        {
            var ficha = await _relatorioRepository.GetFichaFltCompletaAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

            var em = ficha.EncomendaMolde ?? throw new InvalidOperationException("Ficha sem EncomendaMolde.");
            var en = em.Encomenda ?? throw new InvalidOperationException("Encomenda em falta.");
            var cl = en.Cliente ?? throw new InvalidOperationException("Cliente em falta.");
            var mo = em.Molde ?? throw new InvalidOperationException("Molde em falta.");
            var sp = mo.Especificacoes;

            return new FichaContext(ficha, em, en, cl, mo, sp);
        }

        private static void FillHeaderComum(IXLWorksheet ws, FichaContext d)
        {
            ws.Cell("I4").Value = DateTime.Now.ToString("dd/MM/yyyy");
            ws.Cell("C6").Value = d.Molde.Nome;
            ws.Cell("J6").Value = d.Molde.Numero;
        }

        private static void FillBocoCliente(IXLWorksheet ws, FichaContext d)
        {
            ws.Cell("D9").Value = d.Cliente.Nome;
            ws.Cell("D10").Value = d.Encomenda.NomeServicoCliente;
            ws.Cell("E11").Value = d.Encomenda.NumeroProjetoCliente;
            ws.Cell("I11").Value = d.Molde.NumeroMoldeCliente;
            ws.Cell("E12").Value = d.Encomenda.NomeResponsavelCliente;
        }
        private static void SetX(IXLWorksheet ws, string cell, bool condition)
        {
            ws.Cell(cell).Value = condition ? "X" : "";
        }

        private sealed record FichaContext(
            FichaProducao Ficha,
            EncomendaMolde EncomendaMolde,
            Encomenda Encomenda,
            Cliente Cliente,
            Molde Molde,
            EspecificacoesTecnicas? Specs);
    }
}
