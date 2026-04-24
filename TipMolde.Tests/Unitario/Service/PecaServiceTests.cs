using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.DTOs.PecaDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Application.Mappings;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class PecaServiceTests
{
    private Mock<IPecaRepository> _pecaRepository = null!;
    private Mock<IMoldeRepository> _moldeRepository = null!;
    private Mock<ILogger<PecaService>> _logger = null!;
    private PecaService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        // ARRANGE
        _pecaRepository = new Mock<IPecaRepository>();
        _moldeRepository = new Mock<IMoldeRepository>();
        _logger = new Mock<ILogger<PecaService>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PecaProfile>();
        });

        var mapper = mapperConfig.CreateMapper();

        _sut = new PecaService(
            _pecaRepository.Object,
            _moldeRepository.Object,
            mapper,
            _logger.Object);
    }

    private static Molde BuildMolde(int id = 1) => new()
    {
        Molde_id = id,
        Numero = $"M-{id}",
        Numero_cavidades = 1,
        TipoPedido = TipoPedido.NOVO_MOLDE
    };

    private static Peca BuildPeca(int id = 1, int moldeId = 1, string designacao = "Extrator") => new()
    {
        Peca_id = id,
        Designacao = designacao,
        Prioridade = 1,
        MaterialDesignacao = "Aco",
        MaterialRecebido = false,
        Molde_id = moldeId
    };

    [Test(Description = "TPECASRV1 - Create deve persistir peca e devolver DTO quando os dados sao validos.")]
    public async Task CreateAsync_Should_CreatePeca_When_DataIsValid()
    {
        // ARRANGE
        var dto = new CreatePecaDTO
        {
            Designacao = "  Extrator  ",
            Prioridade = 1,
            MaterialDesignacao = "Aco",
            MaterialRecebido = false,
            Molde_id = 1
        };

        _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildMolde());
        _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Extrator", 1)).ReturnsAsync((Peca?)null);
        _pecaRepository.Setup(r => r.AddAsync(It.IsAny<Peca>()))
            .ReturnsAsync((Peca entity) =>
            {
                entity.Peca_id = 12;
                return entity;
            });

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.PecaId.Should().Be(12);
        result.Designacao.Should().Be("Extrator");
        result.Molde_id.Should().Be(1);
        _pecaRepository.Verify(r => r.AddAsync(It.Is<Peca>(p =>
            p.Designacao == "Extrator" &&
            p.Molde_id == 1)), Times.Once);
    }

    [Test(Description = "TPECASRV2 - Create deve falhar quando o molde nao existe.")]
    public async Task CreateAsync_Should_ThrowKeyNotFoundException_When_MoldeDoesNotExist()
    {
        // ARRANGE
        var dto = new CreatePecaDTO
        {
            Designacao = "Extrator",
            Prioridade = 1,
            Molde_id = 7
        };

        _moldeRepository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync((Molde?)null);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "TPECASRV3 - GetAll deve devolver DTOs paginados com PecaId preenchido.")]
    public async Task GetAllAsync_Should_ReturnPagedDtos_When_RequestIsValid()
    {
        // ARRANGE
        var items = new[] { BuildPeca(id: 3), BuildPeca(id: 4, designacao: "Coluna") };
        var paged = new PagedResult<Peca>(items, 2, 1, 10);

        _pecaRepository.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(paged);

        // ACT
        var result = await _sut.GetAllAsync(1, 10);

        // ASSERT
        result.TotalCount.Should().Be(2);
        result.Items.Select(x => x.PecaId).Should().BeEquivalentTo(new[] { 3, 4 });
    }

    [Test(Description = "TPECASRV4 - Update deve preservar MaterialRecebido quando o campo nao e enviado.")]
    public async Task UpdateAsync_Should_PreserveMaterialRecebido_When_FieldIsOmitted()
    {
        // ARRANGE
        var existing = BuildPeca(id: 1, designacao: "Original");
        existing.MaterialRecebido = true;

        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var dto = new UpdatePecaDTO
        {
            Designacao = "Atualizada"
        };

        // ACT
        await _sut.UpdateAsync(1, dto);

        // ASSERT
        _pecaRepository.Verify(r => r.UpdateAsync(It.Is<Peca>(p =>
            p.Peca_id == 1 &&
            p.Designacao == "Atualizada" &&
            p.MaterialRecebido)), Times.Once);
    }

    [Test(Description = "TPECASRV5 - Update deve rejeitar designacao duplicada dentro do mesmo molde.")]
    public async Task UpdateAsync_Should_ThrowArgumentException_When_DesignacaoAlreadyExistsInMolde()
    {
        // ARRANGE
        var existing = BuildPeca(id: 1, moldeId: 7, designacao: "Extrator");
        var duplicate = BuildPeca(id: 2, moldeId: 7, designacao: "Coluna");

        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Coluna", 7)).ReturnsAsync(duplicate);

        var dto = new UpdatePecaDTO
        {
            Designacao = "Coluna"
        };

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(1, dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Ja existe uma peca*");
        _pecaRepository.Verify(r => r.UpdateAsync(It.IsAny<Peca>()), Times.Never);
    }

    [Test(Description = "TPECASRV6 - Delete deve falhar quando a peca nao existe.")]
    public async Task DeleteAsync_Should_ThrowKeyNotFoundException_When_PecaDoesNotExist()
    {
        // ARRANGE
        _pecaRepository.Setup(r => r.GetByIdAsync(88)).ReturnsAsync((Peca?)null);

        // ACT
        Func<Task> act = () => _sut.DeleteAsync(88);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "TPECASRV7 - GetByDesignacao deve devolver DTO quando a peca existe.")]
    public async Task GetByDesignacaoAsync_Should_ReturnMappedResponse_When_PecaExists()
    {
        // ARRANGE
        _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Extrator", 2))
            .ReturnsAsync(BuildPeca(id: 5, moldeId: 2));

        // ACT
        var result = await _sut.GetByDesignacaoAsync("Extrator", 2);

        // ASSERT
        result.Should().NotBeNull();
        result!.PecaId.Should().Be(5);
        result.Molde_id.Should().Be(2);
    }
}
