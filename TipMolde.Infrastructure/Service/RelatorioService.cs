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

        /*public async Task<(byte[] Content, string FileName)> GerarFichaPdfFTLAsync(int fichaId)
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
        }*/

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFLTAsync(int fichaId)
        {
            var ficha = await _relatorioRepository.GetFichaFltCompletaAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

            var em = ficha.EncomendaMolde ?? throw new InvalidOperationException("Ficha sem EncomendaMolde.");
            var en = em.Encomenda ?? throw new InvalidOperationException("Encomenda em falta.");
            var cl = en.Cliente ?? throw new InvalidOperationException("Cliente em falta.");
            var mo = em.Molde ?? throw new InvalidOperationException("Molde em falta.");
            var sp = mo.Especificacoes;

            var templatePath = _configuration["Templates:FichaFLT"]
                ?? throw new InvalidOperationException("Templates:FichaFLT não configurado.");

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


            var imagePath = mo.ImagemCapaPath;

            if (!string.IsNullOrWhiteSpace(imagePath) && !Path.IsPathRooted(imagePath))
            {
                var uploadsRoot = _configuration["Uploads:RootPath"]
                                  ?? @"C:\Users\HP\Documents\TipMolde\Uploads";
                imagePath = Path.Combine(uploadsRoot, imagePath.TrimStart('\\', '/'));
            }

            if (string.IsNullOrWhiteSpace(imagePath))
                throw new InvalidOperationException("Molde sem ImagemCapaPath.");

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Imagem não encontrada no disco: {imagePath}");

            var ext = Path.GetExtension(imagePath).ToLowerInvariant();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                throw new InvalidOperationException($"Formato de imagem não suportado: {ext}");

            var area = ws.Range("B9:J24");
            var pic = ws.AddPicture(imagePath)
                        .MoveTo(area.FirstCell(), 5, 5)
                        .WithSize(550, 320);

            ws.Cell("D28").Value = mo.Numero_cavidades;
            ws.Cell("G28").Value = sp?.MaterialInjecao;
            ws.Cell("J28").Value = sp?.Contracao;
            ws.Cell("E29").Value = sp?.TipoInjecao;
            ws.Cell("J29").Value = sp?.AcabamentoPeca;
            ws.Cell("D30").Value = sp?.MaterialMacho;
            ws.Cell("D31").Value = sp?.MaterialCavidade;
            ws.Cell("D32").Value = sp?.MaterialMovimentos;
            ws.Cell("E34").Value = sp?.SistemaInjecao;

            var cor = (sp?.Cor ?? "").Trim().ToUpperInvariant();

            SetX(ws, "F26", cor == "MONO");
            SetX(ws, "H26", cor == "BI");
            SetX(ws, "J26", cor != "MONO" && cor != "BI" && !string.IsNullOrWhiteSpace(cor));

            SetX(ws, "G33", sp?.LadoFixo == true);
            SetX(ws, "J33", sp?.LadoMovel == true);

            ws.Cell("E38").Value = mo.TipoPedido.ToString();


            ws.Cell("D43").Value = cl.Nome;
            ws.Cell("D44").Value = en.NomeServicoCliente;
            ws.Cell("E45").Value = en.NumeroProjetoCliente;
            ws.Cell("I45").Value = mo.NumeroMoldeCliente;
            ws.Cell("E46").Value = en.NomeResponsavelCliente;
            ws.Cell("B48").Value = em.DataEntregaPrevista.ToString("dd/MM/yyyy");

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"ficha_FTL_{fichaId}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFREAsync(int fichaId)
        {
            var ficha = await _relatorioRepository.GetFichaFltCompletaAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

            var em = ficha.EncomendaMolde ?? throw new InvalidOperationException("Ficha sem EncomendaMolde.");
            var en = em.Encomenda ?? throw new InvalidOperationException("Encomenda em falta.");
            var cl = en.Cliente ?? throw new InvalidOperationException("Cliente em falta.");
            var mo = em.Molde ?? throw new InvalidOperationException("Molde em falta.");
            var sp = mo.Especificacoes;

            var templatePath = _configuration["Templates:FichaFRE"]
                ?? throw new InvalidOperationException("Templates:FichaFRE não configurado.");

            var folhaFre = _configuration["Templates:FolhaFRE"] ?? "FRE - TM.08.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFre);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFre}' não encontrada no template.");

            ws.Cell("I4").Value = DateTime.Now.ToString("dd/MM/yyyy");
            ws.Cell("C6").Value = mo.Nome;
            ws.Cell("J6").Value = mo.Numero;


            var imagePath = mo.ImagemCapaPath;

            if (!string.IsNullOrWhiteSpace(imagePath) && !Path.IsPathRooted(imagePath))
            {
                var uploadsRoot = _configuration["Uploads:RootPath"]
                                  ?? @"C:\Users\HP\Documents\TipMolde\Uploads";
                imagePath = Path.Combine(uploadsRoot, imagePath.TrimStart('\\', '/'));
            }

            if (string.IsNullOrWhiteSpace(imagePath))
                throw new InvalidOperationException("Molde sem ImagemCapaPath.");

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Imagem não encontrada no disco: {imagePath}");

            var ext = Path.GetExtension(imagePath).ToLowerInvariant();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                throw new InvalidOperationException($"Formato de imagem não suportado: {ext}");

            var area = ws.Range("B9:J17");
            var pic = ws.AddPicture(imagePath)
                        .MoveTo(area.FirstCell(), 5, 5)
                        .WithSize(550, 160);

            ws.Cell("D20").Value = mo.Numero_cavidades;
            ws.Cell("G20").Value = sp?.MaterialInjecao;
            ws.Cell("J20").Value = sp?.Contracao;
            ws.Cell("E21").Value = sp?.TipoInjecao;
            ws.Cell("J21").Value = sp?.AcabamentoPeca;
            ws.Cell("E22").Value = sp?.SistemaInjecao;

            var cor = (sp?.Cor ?? "").Trim().ToUpperInvariant();

            SetX(ws, "F18", cor == "MONO");
            SetX(ws, "H18", cor == "BI");
            SetX(ws, "J18", cor != "MONO" && cor != "BI" && !string.IsNullOrWhiteSpace(cor));

            ws.Cell("D26").Value = cl.Nome;
            ws.Cell("D27").Value = en.NomeServicoCliente;
            ws.Cell("E28").Value = en.NumeroProjetoCliente;
            ws.Cell("I28").Value = mo.NumeroMoldeCliente;
            ws.Cell("E29").Value = en.NomeResponsavelCliente;

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"ficha_FRE_{fichaId}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFRMAsync(int fichaId)
        {
            var ficha = await _relatorioRepository.GetFichaFltCompletaAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

            var em = ficha.EncomendaMolde ?? throw new InvalidOperationException("Ficha sem EncomendaMolde.");
            var en = em.Encomenda ?? throw new InvalidOperationException("Encomenda em falta.");
            var cl = en.Cliente ?? throw new InvalidOperationException("Cliente em falta.");
            var mo = em.Molde ?? throw new InvalidOperationException("Molde em falta.");
            var sp = mo.Especificacoes;

            var templatePath = _configuration["Templates:FichaFRM"]
                ?? throw new InvalidOperationException("Templates:FichaFRM não configurado.");

            var folhaFrm = _configuration["Templates:FolhaFRM"] ?? "FRM - TM.09.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFrm);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFrm}' não encontrada no template.");

            ws.Cell("I4").Value = DateTime.Now.ToString("dd/MM/yyyy");
            ws.Cell("C6").Value = mo.Nome;
            ws.Cell("J6").Value = mo.Numero;

            ws.Cell("D9").Value = cl.Nome;
            ws.Cell("D10").Value = en.NomeServicoCliente;
            ws.Cell("E11").Value = en.NumeroProjetoCliente;
            ws.Cell("I11").Value = mo.NumeroMoldeCliente;
            ws.Cell("E12").Value = en.NomeResponsavelCliente;

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"ficha_FRM_{fichaId}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFRAAsync(int fichaId)
        {
            var ficha = await _relatorioRepository.GetFichaFltCompletaAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

            var em = ficha.EncomendaMolde ?? throw new InvalidOperationException("Ficha sem EncomendaMolde.");
            var en = em.Encomenda ?? throw new InvalidOperationException("Encomenda em falta.");
            var cl = en.Cliente ?? throw new InvalidOperationException("Cliente em falta.");
            var mo = em.Molde ?? throw new InvalidOperationException("Molde em falta.");
            var sp = mo.Especificacoes;

            var templatePath = _configuration["Templates:FichaFRA"]
                ?? throw new InvalidOperationException("Templates:FichaFRA não configurado.");

            var folhaFra = _configuration["Templates:FolhaFRA"] ?? "FRA - TM.010.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFra);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFra}' não encontrada no template.");

            ws.Cell("I4").Value = DateTime.Now.ToString("dd/MM/yyyy");
            ws.Cell("C6").Value = mo.Nome;
            ws.Cell("J6").Value = mo.Numero;

            ws.Cell("D9").Value = cl.Nome;
            ws.Cell("D10").Value = en.NomeServicoCliente;
            ws.Cell("E11").Value = en.NumeroProjetoCliente;
            ws.Cell("I11").Value = mo.NumeroMoldeCliente;
            ws.Cell("E12").Value = en.NomeResponsavelCliente;

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"ficha_FRA_{fichaId}.xlsx");
        }

        public async Task<(byte[] Content, string FileName)> GerarFichaExcelFOPAsync(int fichaId)
        {
            var ficha = await _relatorioRepository.GetFichaFltCompletaAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

            var em = ficha.EncomendaMolde ?? throw new InvalidOperationException("Ficha sem EncomendaMolde.");
            var en = em.Encomenda ?? throw new InvalidOperationException("Encomenda em falta.");
            var cl = en.Cliente ?? throw new InvalidOperationException("Cliente em falta.");
            var mo = em.Molde ?? throw new InvalidOperationException("Molde em falta.");
            var sp = mo.Especificacoes;

            var templatePath = _configuration["Templates:FichaFOP"]
                ?? throw new InvalidOperationException("Templates:FichaFOP não configurado.");

            var folhaFop = _configuration["Templates:FolhaFOP"] ?? "FOP - TM.07.05";

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template não encontrado: {templatePath}");

            using var wb = new XLWorkbook(templatePath);
            var ws = wb.Worksheet(folhaFop);
            if (ws == null)
                throw new KeyNotFoundException($"Folha '{folhaFop}' não encontrada no template.");

            ws.Cell("I4").Value = DateTime.Now.ToString("dd/MM/yyyy");
            ws.Cell("C6").Value = mo.Nome;
            ws.Cell("J6").Value = mo.Numero;

            ws.Cell("D9").Value = cl.Nome;
            ws.Cell("D10").Value = en.NomeServicoCliente;
            ws.Cell("E11").Value = en.NumeroProjetoCliente;
            ws.Cell("I11").Value = mo.NumeroMoldeCliente;
            ws.Cell("E12").Value = en.NomeResponsavelCliente;

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return (ms.ToArray(), $"ficha_FOP_{fichaId}.xlsx");
        }

        private static void SetX(IXLWorksheet ws, string cell, bool condition)
        {
            ws.Cell(cell).Value = condition ? "X" : "";
        }
    }
}
