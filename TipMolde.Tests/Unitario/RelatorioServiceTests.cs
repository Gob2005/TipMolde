using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Fichas.IFichaDocumento;
using TipMolde.Core.Models.Comercio;
using TipMolde.Core.Models.Fichas;
using TipMolde.Core.Models.Producao;
using TipMolde.Infrastructure.DB;
using TipMolde.Infrastructure.Repositorio;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    public class RelatorioServiceTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private static RelatorioService CreateSut(ApplicationDbContext ctx)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Templates:FichaFLT"] = @"C:\Users\HP\Documents\TipMolde\Templates\FLT.xlsx",
                    ["Templates:FichaFRE"] = @"C:\Users\HP\Documents\TipMolde\Templates\FRE.xlsx",
                    ["Templates:FichaFRM"] = @"C:\Users\HP\Documents\TipMolde\Templates\FRM.xlsx",
                    ["Templates:FichaFRA"] = @"C:\Users\HP\Documents\TipMolde\Templates\FRA.xlsx",
                    ["Templates:FichaFOP"] = @"C:\Users\HP\Documents\TipMolde\Templates\FOP.xlsx",
                    ["Templates:FolhaFLT"] = "FLT - TM.04.05",
                    ["Templates:FolhaFRE"] = "FRE - TM.08.05",
                    ["Templates:FolhaFRM"] = "FRM - TM.09.05",
                    ["Templates:FolhaFRA"] = "FRA - TM.010.05",
                    ["Templates:FolhaFOP"] = "FOP - TM.07.05"
                })
                .Build();
            var repo = new RelatorioRepository(ctx);
            var fichaDocServiceMock = new Mock<IFichaDocumentoService>();
            return new RelatorioService(repo, config,fichaDocServiceMock.Object);
        }

        private static async Task<int> SeedMoldeAsync(ApplicationDbContext ctx, string numero = "M-001")
        {
            var molde = new Molde
            {
                Numero = numero,
                Nome = "Molde Teste",
                NumeroMoldeCliente = "Molde Teste - 001",
                Descricao = "Descricao teste",
                Numero_cavidades = 4,
                TipoPedido = TipoPedido.NOVO_MOLDE
            };

            await ctx.Moldes.AddAsync(molde);
            await ctx.SaveChangesAsync();
            return molde.Molde_id;
        }

        private static async Task<int> SeedFichaAsync(ApplicationDbContext ctx, int fichaId = 1)
        {
            var cliente = new Cliente
            {
                Nome = "Cliente X",
                NIF = $"123456{fichaId:000}",
                Sigla = $"CX{fichaId}"
            };
            await ctx.Clientes.AddAsync(cliente);
            await ctx.SaveChangesAsync();

            var encomenda = new Encomenda
            {
                NumeroEncomendaCliente = $"ENC-{fichaId:000}",
                NomeServicoCliente = $"Serviço Teste",
                NumeroProjetoCliente = $"PRJ-{fichaId:000}",
                NomeResponsavelCliente = $"Responsável Gonçalo",
                Cliente_id = cliente.Cliente_id
            };
            await ctx.Encomendas.AddAsync(encomenda);
            await ctx.SaveChangesAsync();

            var molde = new Molde
            {
                Numero = $"M-{fichaId:000}",
                Nome = $"Molde {fichaId:000}",
                NumeroMoldeCliente = "Molde Teste - 001",
                ImagemCapaPath = @"C:\Users\HP\Documents\TipMolde\Templates\Imagem_Template.png",
                Numero_cavidades = 2,
                TipoPedido = TipoPedido.NOVO_MOLDE
            };
            await ctx.Moldes.AddAsync(molde);
            await ctx.SaveChangesAsync();

            var specs = new EspecificacoesTecnicas
            {
                Molde_id = molde.Molde_id,
                TipoInjecao = "Hot Runner",
                SistemaInjecao = "Canal Quente",
                MaterialInjecao = "ABS",
                MaterialMacho = "AISI P20",
                MaterialCavidade = "H13",
                MaterialMovimentos = "AISI 420",
                AcabamentoPeca = "Polido",
                Contracao = 1.20m,
                Cor = CorMolde.BICOLOR,
                LadoFixo = true,
                LadoMovel = true
            };
            await ctx.EspecificacoesTecnicas.AddAsync(specs);
            await ctx.SaveChangesAsync();

            var link = new EncomendaMolde
            {
                Encomenda_id = encomenda.Encomenda_id,
                Molde_id = molde.Molde_id,
                Quantidade = 1,
                Prioridade = 1,
                DataEntregaPrevista = DateTime.UtcNow.Date.AddDays(10)
            };
            await ctx.EncomendasMoldes.AddAsync(link);
            await ctx.SaveChangesAsync();

            var ficha = new FichaProducao
            {
                FichaProducao_id = fichaId,
                Tipo = TipoFicha.FLT,
                DataCriacao = DateTime.UtcNow,
                EncomendaMolde_id = link.EncomendaMolde_id
            };
            await ctx.FichasProducao.AddAsync(ficha);
            await ctx.SaveChangesAsync();

            return ficha.FichaProducao_id;
        }

        [Fact]
        public async Task GerarCicloVidaMoldePdfAsync_ComMoldeExistente_GeraPdf()
        {
            // Arrange
            await using var ctx = CreateContext();
            var moldeId = await SeedMoldeAsync(ctx, "M-010");
            var sut = CreateSut(ctx);

            // Act
            var result = await sut.GerarCicloVidaMoldePdfAsync(moldeId);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var excelPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(excelPath, result.Content);

            // Assert
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".pdf", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ciclo_vida_molde_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 100);
        }

        [Fact]
        public async Task GerarCicloVidaMoldePdfAsync_ComMoldeInexistente_LancaExcecao()
        {
            // Arrange
            await using var ctx = CreateContext();
            var sut = CreateSut(ctx);

            // Act
            var act = () => sut.GerarCicloVidaMoldePdfAsync(999);

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(act);
        }

        [Fact]
        public async Task GerarFichaExcelFLTAsync_ComFichaExistente_GeraExcel()
        {
            // Arrange
            await using var ctx = CreateContext();
            var fichaId = await SeedFichaAsync(ctx, 30);
            var sut = CreateSut(ctx);

            // Act
            var result = await sut.GerarFichaExcelFLTAsync(fichaId, 1);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var excelPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(excelPath, result.Content);

            // Assert
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".xlsx", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ficha_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 50);
        }

        [Fact]
        public async Task GerarFichaExcelFREAsync_ComFichaExistente_GeraExcel()
        {
            // Arrange
            await using var ctx = CreateContext();
            var fichaId = await SeedFichaAsync(ctx, 30);
            var sut = CreateSut(ctx);

            // Act
            var result = await sut.GerarFichaExcelFREAsync(fichaId, 1);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var excelPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(excelPath, result.Content);

            // Assert
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".xlsx", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ficha_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 50);
        }

        [Fact]
        public async Task GerarFichaExcelFRMAsync_ComFichaExistente_GeraExcel()
        {
            // Arrange
            await using var ctx = CreateContext();
            var fichaId = await SeedFichaAsync(ctx, 30);
            var sut = CreateSut(ctx);

            // Act
            var result = await sut.GerarFichaExcelFRMAsync(fichaId, 1);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var excelPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(excelPath, result.Content);

            // Assert
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".xlsx", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ficha_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 50);
        }

        [Fact]
        public async Task GerarFichaExcelFRAAsync_ComFichaExistente_GeraExcel()
        {
            // Arrange
            await using var ctx = CreateContext();
            var fichaId = await SeedFichaAsync(ctx, 30);
            var sut = CreateSut(ctx);

            // Act
            var result = await sut.GerarFichaExcelFRAAsync(fichaId, 1);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var excelPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(excelPath, result.Content);

            // Assert
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".xlsx", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ficha_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 50);
        }

        [Fact]
        public async Task GerarFichaExcelFOPAsync_ComFichaExistente_GeraExcel()
        {
            // Arrange
            await using var ctx = CreateContext();
            var fichaId = await SeedFichaAsync(ctx, 30);
            var sut = CreateSut(ctx);

            // Act
            var result = await sut.GerarFichaExcelFOPAsync(fichaId, 1);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var excelPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(excelPath, result.Content);

            // Assert
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".xlsx", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ficha_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 50);
        }

        [Fact]
        public async Task GerarFichaExcelFLTAsync_ComFichaInexistente_LancaExcecao()
        {
            // Arrange
            await using var ctx = CreateContext();
            var sut = CreateSut(ctx);

            // Act
            var act = () => sut.GerarFichaExcelFLTAsync(999, 1);

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(act);
        }
    }
}
