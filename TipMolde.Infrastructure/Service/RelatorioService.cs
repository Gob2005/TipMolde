using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TipMolde.Application.Dtos.RelatorioDto;
using TipMolde.Application.Interface.Fichas.IFichaDocumento;
using TipMolde.Application.Interface.Relatorios;
using TipMolde.Domain.Enums;

namespace TipMolde.Infrastructure.Service
{
    /// <summary>
    /// Gera artefactos documentais e KPI do modulo de relatorios.
    /// </summary>
    /// <remarks>
    /// Este servico orquestra templates Excel, PDF e persistencia documental.
    /// A regra de rastreabilidade obriga que o nome devolvido ao cliente seja exatamente o
    /// mesmo que fica versionado em FichaDocumento.
    /// </remarks>
    public class RelatorioService : IRelatorioService
    {
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly IRelatorioRepository _relatorioRepository;
        private readonly IConfiguration _configuration;
        private readonly IFichaDocumentoService _fichaDocumentoService;

        /// <summary>
        /// Construtor de RelatorioService.
        /// </summary>
        /// <param name="relatorioRepository">Repositorio de leitura especializado para relatorios e indicadores.</param>
        /// <param name="configuration">Configuracao de templates, folhas e paths tecnicos.</param>
        /// <param name="fichaDocumentoService">Servico responsavel por versionar e persistir os ficheiros gerados.</param>
        public RelatorioService(
            IRelatorioRepository relatorioRepository,
            IConfiguration configuration,
            IFichaDocumentoService fichaDocumentoService)
        {
            _relatorioRepository = relatorioRepository;
            _configuration = configuration;
            _fichaDocumentoService = fichaDocumentoService;
        }

        /// <summary>
        /// Gera o relatorio PDF do ciclo de vida completo de um molde.
        /// </summary>
        /// <remarks>
        /// Agrega informacao comercial, de desenho e de producao para cumprir o requisito RF-RL-01.
        /// Se alguma fonte transversal relevante nao existir, o relatorio deve sinalizar a falta de dados
        /// em vez de aparentar falsamente que o ciclo de vida esta completo.
        /// </remarks>
        /// <param name="moldeId">Identificador interno do molde.</param>
        /// <returns>Conteudo binario do PDF e nome do ficheiro gerado.</returns>
        public async Task<(byte[] Content, string FileName)> GerarCicloVidaMoldePdfAsync(int moldeId)
        {
            var relatorio = await _relatorioRepository.ObterMoldeCicloVidaAsync(moldeId)
                ?? throw new KeyNotFoundException($"Molde {moldeId} nao encontrado.");

            QuestPDF.Settings.License = LicenseType.Community;

            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(24);

                    page.Header()
                        .Text($"Relatorio do Ciclo de Vida - Molde {relatorio.NumeroMolde}")
                        .SemiBold()
                        .FontSize(18);

                    page.Content().Column(column =>
                    {
                        column.Spacing(6);

                        AddSection(column, "Identificacao", [
                            ("Numero interno", relatorio.NumeroMolde),
                            ("Numero cliente", relatorio.NumeroMoldeCliente),
                            ("Nome", relatorio.NomeMolde),
                            ("Descricao", relatorio.DescricaoMolde),
                            ("Tipo de pedido", relatorio.TipoPedido.ToString()),
                            ("Numero de cavidades", relatorio.NumeroCavidades.ToString())
                        ]);

                        AddSection(column, "Comercial", [
                            ("Cliente", relatorio.ClienteNome),
                            ("Encomenda cliente", relatorio.NumeroEncomendaCliente),
                            ("Projeto cliente", relatorio.NumeroProjetoCliente),
                            ("Responsavel cliente", relatorio.NomeResponsavelCliente),
                            ("Data de registo da encomenda", FormatDate(relatorio.DataRegistoEncomenda)),
                            ("Data de entrega prevista", FormatDate(relatorio.DataEntregaPrevista))
                        ]);

                        AddSection(column, "Desenho", [
                            ("Projetos registados", relatorio.TotalProjetos.ToString()),
                            ("Revisoes registadas", relatorio.TotalRevisoes.ToString()),
                            ("Ultima revisao", FormatDate(relatorio.UltimaRevisaoEm))
                        ]);

                        AddSection(column, "Producao", [
                            ("Total de pecas", relatorio.TotalPecas.ToString()),
                            ("Material pendente", relatorio.MaterialPendente.ToString()),
                            ("Pecas com movimento em maquinacao", relatorio.Maquinacao.ToString()),
                            ("Pecas com movimento em erosao", relatorio.Erosao.ToString()),
                            ("Pecas com movimento em montagem", relatorio.Montagem.ToString()),
                            ("Registos em trabalho", relatorio.EmTrabalho.ToString()),
                            ("Pecas concluidas", relatorio.Concluidas.ToString()),
                            ("Percentagem de conclusao", $"{relatorio.PercentagemConclusao:N2}%")
                        ]);
                    });
                });
            }).GeneratePdf();

            return (bytes, $"ciclo_vida_molde_{moldeId}.pdf");
        }

        /// <summary>
        /// Calcula os KPI do ciclo de vida produtivo de um molde.
        /// </summary>
        /// <param name="moldeId">Identificador interno do molde.</param>
        /// <returns>DTO com os principais indicadores agregados do molde.</returns>
        public async Task<MoldeCicloVidaDashboardDto> ObterDashboardMoldeAsync(int moldeId)
        {
            var relatorio = await _relatorioRepository.ObterMoldeCicloVidaAsync(moldeId)
                ?? throw new KeyNotFoundException($"Molde {moldeId} nao encontrado.");

            return new MoldeCicloVidaDashboardDto
            {
                MoldeId = relatorio.MoldeId,
                NumeroMolde = relatorio.NumeroMolde,
                TotalPecas = relatorio.TotalPecas,
                Maquinacao = relatorio.Maquinacao,
                Erosao = relatorio.Erosao,
                Montagem = relatorio.Montagem,
                EmTrabalho = relatorio.EmTrabalho,
                Concluidas = relatorio.Concluidas,
                MaterialPendente = relatorio.MaterialPendente,
                PercentagemConclusao = relatorio.PercentagemConclusao
            };
        }

        /// <summary>
        /// Gera a ficha FLT oficial a partir do template configurado.
        /// </summary>
        /// <remarks>
        /// A FLT nao e uma ficha editavel persistida.
        /// O documento e gerado diretamente a partir da relacao Encomenda-Molde e por isso
        /// nao entra no fluxo de versionamento de FichaDocumento.
        /// </remarks>
        /// <param name="encomendaMoldeId">Identificador da relacao Encomenda-Molde usada como contexto da FLT.</param>
        /// <param name="userId">Identificador do utilizador responsavel pela geracao.</param>
        /// <returns>Conteudo binario do Excel e nome final do ficheiro gerado.</returns>
        public Task<(byte[] Content, string FileName)> GerarFichaExcelFLTAsync(int encomendaMoldeId, int userId)
        {
            return GerarFichaExcelSemVersionamentoAsync(
                encomendaMoldeId,
                "Templates:FichaFLT",
                "Templates:FolhaFLT",
                "FLT - TM.04.05",
                $"ficha_FLT_{encomendaMoldeId}.xlsx",
                _relatorioRepository.ObterFltRelatorioBaseAsync,
                FillFltBody);
        }

        /// <summary>
        /// Gera a ficha FRE oficial a partir do template configurado.
        /// </summary>
        /// <param name="fichaId">Identificador interno da ficha de producao.</param>
        /// <param name="userId">Identificador do utilizador responsavel pela geracao.</param>
        /// <returns>Conteudo binario do Excel e nome final versionado do ficheiro.</returns>

        public Task<(byte[] Content, string FileName)> GerarFichaExcelFREAsync(int fichaId, int userId)
        {
            return GerarFichaExcelAsync(
                fichaId,
                userId,
                "Templates:FichaFRE",
                "Templates:FolhaFRE",
                "FRE - TM.08.05",
                $"ficha_FRE_{fichaId}.xlsx",
                FillFreBody);
        }

        /// <summary>
        /// Gera a ficha FRM oficial a partir do template configurado.
        /// </summary>
        /// <remarks>
        /// No estado atual, este metodo so cumpre o criterio documental.
        /// Para cumprir o criterio funcional na totalidade, precisa de uma projection propria com
        /// os registos de melhoria/alteracao efetivamente persistidos no dominio.
        /// </remarks>
        /// <param name="fichaId">Identificador interno da ficha de producao.</param>
        /// <param name="userId">Identificador do utilizador responsavel pela geracao.</param>
        /// <returns>Conteudo binario do Excel e nome final versionado do ficheiro.</returns>

        public Task<(byte[] Content, string FileName)> GerarFichaExcelFRMAsync(int fichaId, int userId)
        {
            return GerarFichaExcelAsync(
                fichaId,
                userId,
                "Templates:FichaFRM",
                "Templates:FolhaFRM",
                "FRM - TM.09.05",
                $"ficha_FRM_{fichaId}.xlsx",
                FillBlocoCliente);
        }

        public Task<(byte[] Content, string FileName)> GerarFichaExcelFRAAsync(int fichaId, int userId)
        {
            return GerarFichaExcelAsync(
                fichaId,
                userId,
                "Templates:FichaFRA",
                "Templates:FolhaFRA",
                "FRA - TM.010.05",
                $"ficha_FRA_{fichaId}.xlsx",
                FillBlocoCliente);
        }

        public Task<(byte[] Content, string FileName)> GerarFichaExcelFOPAsync(int fichaId, int userId)
        {
            return GerarFichaExcelAsync(
                fichaId,
                userId,
                "Templates:FichaFOP",
                "Templates:FolhaFOP",
                "FOP - TM.07.05",
                $"ficha_FOP_{fichaId}.xlsx",
                FillBlocoCliente);
        }

        /// <summary>
        /// Gera uma ficha Excel oficial a partir de um template configurado.
        /// </summary>
        /// <remarks>
        /// Fluxo critico:
        /// 1. Carrega o read-model da ficha necessario ao preenchimento.
        /// 2. Valida a configuracao do template e da folha.
        /// 3. Preenche cabecalho e corpo especifico da ficha.
        /// 4. Persiste o artefacto gerado em FichaDocumento.
        /// 5. Devolve ao cliente o mesmo nome versionado que ficou auditado.
        /// </remarks>
        /// <param name="fichaId">Identificador interno da ficha de producao.</param>
        /// <param name="userId">Identificador do utilizador responsavel pela geracao.</param>
        /// <param name="templateConfigKey">Chave de configuracao do caminho do template Excel.</param>
        /// <param name="folhaConfigKey">Chave de configuracao do nome da folha a usar.</param>
        /// <param name="defaultSheetName">Nome por defeito da folha quando a configuracao nao existe.</param>
        /// <param name="fileName">Nome base do ficheiro a persistir.</param>
        /// <param name="fillBody">Acao responsavel por preencher o corpo especifico da ficha.</param>
        /// <returns>Conteudo binario do Excel e nome final versionado do ficheiro.</returns>
        private async Task<(byte[] Content, string FileName)> GerarFichaExcelSemVersionamentoAsync(
            int contextId,
            string templateConfigKey,
            string folhaConfigKey,
            string defaultSheetName,
            string fileName,
            Func<int, Task<FichaRelatorioBaseDto?>> loadContext,
            Action<IXLWorksheet, FichaRelatorioBaseDto> fillBody)
        {
            var context = await loadContext(contextId)
                ?? throw new KeyNotFoundException($"Contexto {contextId} nao encontrado.");

            using var workbook = LoadWorkbook(templateConfigKey, folhaConfigKey, defaultSheetName, out var worksheet);
            FillHeaderComum(worksheet, context);
            fillBody(worksheet, context);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return (ms.ToArray(), fileName);
        }

        private async Task<(byte[] Content, string FileName)> GerarFichaExcelAsync(
            int fichaId,
            int userId,
            string templateConfigKey,
            string folhaConfigKey,
            string defaultSheetName,
            string fileName,
            Action<IXLWorksheet, FichaRelatorioBaseDto> fillBody)
        {
            var context = await _relatorioRepository.ObterFichaRelatorioBaseAsync(fichaId)
                ?? throw new KeyNotFoundException($"Ficha {fichaId} nao encontrada.");

            using var workbook = LoadWorkbook(templateConfigKey, folhaConfigKey, defaultSheetName, out var worksheet);

            FillHeaderComum(worksheet, context);
            fillBody(worksheet, context);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var bytes = ms.ToArray();

            var documento = await _fichaDocumentoService.GuardarGeradoAsync(
                fichaId,
                bytes,
                fileName,
                ExcelContentType,
                userId,
                "SISTEMA");

            return (bytes, documento.NomeFicheiro);
        }

        /// <summary>
        /// Carrega o workbook e valida a configuracao do template Excel.
        /// </summary>
        /// <param name="templateConfigKey">Chave de configuracao do caminho do template Excel.</param>
        /// <param name="folhaConfigKey">Chave de configuracao do nome da folha a usar.</param>
        /// <param name="defaultSheetName">Nome por defeito da folha quando a configuracao nao existe.</param>
        /// <param name="worksheet">Folha carregada pronta a ser preenchida.</param>
        /// <returns>Workbook carregado a partir do template configurado.</returns>
        private XLWorkbook LoadWorkbook(
            string templateConfigKey,
            string folhaConfigKey,
            string defaultSheetName,
            out IXLWorksheet worksheet)
        {
            var templatePath = _configuration[templateConfigKey]
                ?? throw new InvalidOperationException($"{templateConfigKey} nao configurado.");

            var worksheetName = _configuration[folhaConfigKey] ?? defaultSheetName;

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template nao encontrado: {templatePath}");

            var workbook = new XLWorkbook(templatePath);
            worksheet = workbook.Worksheet(worksheetName)
                ?? throw new KeyNotFoundException($"Folha '{worksheetName}' nao encontrada no template.");

            return workbook;
        }

        private void FillFltBody(IXLWorksheet ws, FichaRelatorioBaseDto context)
        {
            var imagePath = ResolveImagePath(context.ImagemCapaPath);
            ws.AddPicture(imagePath)
                .MoveTo(ws.Range("B9:J24").FirstCell(), 5, 5)
                .WithSize(550, 320);

            ws.Cell("D28").Value = context.NumeroCavidades;
            ws.Cell("G28").Value = context.MaterialInjecao;
            ws.Cell("J28").Value = context.Contracao;
            ws.Cell("E29").Value = context.TipoInjecao;
            ws.Cell("J29").Value = context.AcabamentoPeca;
            ws.Cell("D30").Value = context.MaterialMacho;
            ws.Cell("D31").Value = context.MaterialCavidade;
            ws.Cell("D32").Value = context.MaterialMovimentos;
            ws.Cell("E34").Value = context.SistemaInjecao;

            SetX(ws, "F26", context.Cor == CorMolde.MONOCOLOR);
            SetX(ws, "H26", context.Cor == CorMolde.BICOLOR);
            SetX(ws, "J26", context.Cor == CorMolde.OUTRO);
            SetX(ws, "G33", context.LadoFixo);
            SetX(ws, "J33", context.LadoMovel);

            ws.Cell("E38").Value = context.TipoPedido.ToString();
            FillBlocoClienteFlt(ws, context);
            ws.Cell("B48").Value = context.DataEntregaPrevista.ToString("dd/MM/yyyy");
        }

        private void FillFreBody(IXLWorksheet ws, FichaRelatorioBaseDto context)
        {
            var imagePath = ResolveImagePath(context.ImagemCapaPath);
            ws.AddPicture(imagePath)
                .MoveTo(ws.Range("B9:J17").FirstCell(), 5, 5)
                .WithSize(550, 160);

            ws.Cell("D20").Value = context.NumeroCavidades;
            ws.Cell("G20").Value = context.MaterialInjecao;
            ws.Cell("J20").Value = context.Contracao;
            ws.Cell("E21").Value = context.TipoInjecao;
            ws.Cell("J21").Value = context.AcabamentoPeca;
            ws.Cell("E22").Value = context.SistemaInjecao;

            SetX(ws, "F18", context.Cor == CorMolde.MONOCOLOR);
            SetX(ws, "H18", context.Cor == CorMolde.BICOLOR);
            SetX(ws, "J18", context.Cor == CorMolde.OUTRO);

            FillBlocoClienteFre(ws, context);
        }

        private static void FillHeaderComum(IXLWorksheet ws, FichaRelatorioBaseDto context)
        {
            ws.Cell("I4").Value = DateTime.UtcNow.ToString("dd/MM/yyyy");
            ws.Cell("C6").Value = context.MoldeNome;
            ws.Cell("J6").Value = context.MoldeNumero;
        }

        private static void FillBlocoCliente(IXLWorksheet ws, FichaRelatorioBaseDto context)
        {
            ws.Cell("D9").Value = context.ClienteNome;
            ws.Cell("D10").Value = context.NomeServicoCliente;
            ws.Cell("E11").Value = context.NumeroProjetoCliente;
            ws.Cell("I11").Value = context.NumeroMoldeCliente;
            ws.Cell("E12").Value = context.NomeResponsavelCliente;
        }

        private static void FillBlocoClienteFlt(IXLWorksheet ws, FichaRelatorioBaseDto context)
        {
            ws.Cell("D43").Value = context.ClienteNome;
            ws.Cell("D44").Value = context.NomeServicoCliente;
            ws.Cell("E45").Value = context.NumeroProjetoCliente;
            ws.Cell("I45").Value = context.NumeroMoldeCliente;
            ws.Cell("E46").Value = context.NomeResponsavelCliente;
        }

        private static void FillBlocoClienteFre(IXLWorksheet ws, FichaRelatorioBaseDto context)
        {
            ws.Cell("D26").Value = context.ClienteNome;
            ws.Cell("D27").Value = context.NomeServicoCliente;
            ws.Cell("E28").Value = context.NumeroProjetoCliente;
            ws.Cell("I28").Value = context.NumeroMoldeCliente;
            ws.Cell("E29").Value = context.NomeResponsavelCliente;
        }

        /// <summary>
        /// Resolve o caminho fisico da imagem de capa do molde.
        /// </summary>
        /// <remarks>
        /// Porque: os templates Excel dependem de um ficheiro fisico valido.
        /// Risco: se o path relativo for resolvido contra uma raiz errada, a exportacao falha
        /// ou incorpora uma imagem incorreta no documento oficial.
        /// </remarks>
        /// <param name="imagePathRaw">Caminho absoluto ou relativo guardado no sistema.</param>
        /// <returns>Caminho fisico absoluto pronto a ser consumido pelo ClosedXML.</returns>
        private string ResolveImagePath(string? imagePathRaw)
        {
            if (string.IsNullOrWhiteSpace(imagePathRaw))
                throw new ArgumentNullException(nameof(imagePathRaw), "Imagem sem caminho.");

            var imagePath = imagePathRaw;
            if (!Path.IsPathRooted(imagePath))
            {
                var uploadsRoot = _configuration["Uploads:RootPath"];
                if (string.IsNullOrWhiteSpace(uploadsRoot))
                    throw new InvalidOperationException("Uploads:RootPath nao configurado.");

                imagePath = Path.Combine(uploadsRoot, imagePath.TrimStart('\\', '/'));
            }

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Imagem nao encontrada: {imagePath}");

            var ext = Path.GetExtension(imagePath).ToLowerInvariant();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                throw new InvalidOperationException("Formato de imagem nao suportado.");

            return imagePath;
        }

        private static void SetX(IXLWorksheet ws, string cell, bool condition)
        {
            ws.Cell(cell).Value = condition ? "X" : string.Empty;
        }

        private static void AddSection(ColumnDescriptor column, string title, IEnumerable<(string Label, string? Value)> rows)
        {
            column.Item().PaddingTop(8).Text(title).Bold().FontSize(14);

            foreach (var row in rows)
                column.Item().Text($"{row.Label}: {Normalize(row.Value)}");
        }

        private static string FormatDate(DateTime? value)
        {
            return value?.ToString("dd/MM/yyyy") ?? "n/d";
        }

        private static string Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "n/d" : value.Trim();
        }
    }
}
