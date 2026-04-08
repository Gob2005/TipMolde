using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Enums;
using TipMolde.Core.Models.Comercio;
using TipMolde.Core.Models.Fichas;
using TipMolde.Core.Models.Producao;
using TipMolde.Infrastructure.DB;
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

        private static async Task SeedMoldeAsync(ApplicationDbContext ctx, int moldeId = 1)
        {
            var molde = new Molde
            {
                Molde_id = moldeId,
                Numero = $"M-{moldeId}",
                Nome = "Molde Teste",
                Descricao = "Descricao teste",
                Numero_cavidades = 4,
                TipoPedido = TipoPedido.NOVO_MOLDE
            };

            await ctx.Moldes.AddAsync(molde);
            await ctx.SaveChangesAsync();
        }

        private static async Task<int> SeedFichaAsync(ApplicationDbContext ctx, int fichaId = 1)
        {
            var cliente = new Cliente { Nome = "Cliente X", NIF = "123456789", Sigla = "CX" };
            await ctx.Clientes.AddAsync(cliente);
            await ctx.SaveChangesAsync();

            var encomenda = new Encomenda
            {
                NumeroEncomendaCliente = "ENC-001",
                Cliente_id = cliente.Cliente_id
            };
            await ctx.Encomendas.AddAsync(encomenda);
            await ctx.SaveChangesAsync();

            var molde = new Molde
            {
                Numero = "M-001",
                Nome = "Molde 001",
                Numero_cavidades = 2,
                TipoPedido = TipoPedido.NOVO_MOLDE
            };
            await ctx.Moldes.AddAsync(molde);
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
                DataGeracao = DateTime.UtcNow,
                EncomendaMolde_id = link.EncomendaMolde_id
            };
            await ctx.FichasProducao.AddAsync(ficha);
            await ctx.SaveChangesAsync();

            return ficha.FichaProducao_id;
        }

        [Fact]
        public async Task GerarCicloVidaMoldePdfAsync_ComMoldeExistente_GeraPdf()
        {
            await using var ctx = CreateContext();
            await SeedMoldeAsync(ctx, 10);

            var sut = new RelatorioService(ctx);

            var result = await sut.GerarCicloVidaMoldePdfAsync(10);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var fullPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(fullPath, result.Content);

            Assert.True(File.Exists(fullPath));

            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".pdf", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ciclo_vida_molde_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 100);
        }

        [Fact]
        public async Task GerarCicloVidaMoldePdfAsync_ComMoldeInexistente_LancaExcecao()
        {
            await using var ctx = CreateContext();
            var sut = new RelatorioService(ctx);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GerarCicloVidaMoldePdfAsync(999));
        }

        [Fact]
        public async Task GerarFichaPdfFTLAsync_ComFichaExistente_GeraPdf()
        {
            await using var ctx = CreateContext();
            var fichaId = await SeedFichaAsync(ctx, 20);

            var sut = new RelatorioService(ctx);

            var result = await sut.GerarFichaPdfFTLAsync(fichaId);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var fullPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(fullPath, result.Content);

            Assert.True(File.Exists(fullPath));

            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".pdf", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ficha_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 100);
        }

        [Fact]
        public async Task GerarFichaExcelFTLAsync_ComFichaExistente_GeraExcel()
        {
            await using var ctx = CreateContext();
            var fichaId = await SeedFichaAsync(ctx, 30);

            var sut = new RelatorioService(ctx);

            var result = await sut.GerarFichaExcelFTLAsync(fichaId);

            var outDir = @"C:\Users\HP\Documents\TipMolde\RelatoriosMock";
            Directory.CreateDirectory(outDir);

            var excelPath = Path.Combine(outDir, result.FileName);
            await File.WriteAllBytesAsync(excelPath, result.Content);

            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
            Assert.EndsWith(".xlsx", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith("ficha_", result.FileName, StringComparison.OrdinalIgnoreCase);
            Assert.True(result.Content.Length > 50);
        }

        [Fact]
        public async Task GerarFichaPdfFTLAsync_ComFichaInexistente_LancaExcecao()
        {
            await using var ctx = CreateContext();
            var sut = new RelatorioService(ctx);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GerarFichaPdfFTLAsync(999));
        }

        [Fact]
        public async Task GerarFichaExcelFTLAsync_ComFichaInexistente_LancaExcecao()
        {
            await using var ctx = CreateContext();
            var sut = new RelatorioService(ctx);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GerarFichaExcelFTLAsync(999));
        }
    }
}
