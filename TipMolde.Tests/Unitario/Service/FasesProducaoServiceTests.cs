using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.Dtos.FasesProducaoDto;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Application.Mappings;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class FasesProducaoServiceTests
{
    private Mock<IFasesProducaoRepository> _repository = null!;
    private Mock<ILogger<FasesProducaoService>> _logger = null!;
    private IMapper _mapper = null!;
    private FasesProducaoService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IFasesProducaoRepository>();
        _logger = new Mock<ILogger<FasesProducaoService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<FasesProducaoProfile>());
        _mapper = config.CreateMapper();

        _sut = new FasesProducaoService(_repository.Object, _mapper, _logger.Object);
    }

    private static FasesProducao BuildFase(int id = 1, NomeFases nome = NomeFases.MAQUINACAO, string? descricao = "Descricao")
    {
        return new FasesProducao
        {
            Fases_producao_id = id,
            Nome = nome,
            Descricao = descricao
        };
    }

    [Test(Description = "TFPSERV1 - Create deve criar fase quando o nome e unico.")]
    public async Task Create_Should_CreateFase_When_NomeIsUnique()
    {
        // ARRANGE
        var dto = new CreateFasesProducaoDto { Nome = NomeFases.MAQUINACAO, Descricao = "Descricao" };
        _repository.Setup(r => r.GetByNomeAsync(NomeFases.MAQUINACAO)).ReturnsAsync((FasesProducao?)null);
        _repository.Setup(r => r.CreateAsync(It.IsAny<FasesProducao>()))
            .ReturnsAsync((FasesProducao fase) =>
            {
                fase.Fases_producao_id = 1;
                return fase;
            });

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.FasesProducao_id.Should().Be(1);
        result.Nome.Should().Be(NomeFases.MAQUINACAO);
        _repository.Verify(r => r.CreateAsync(It.Is<FasesProducao>(f =>
            f.Nome == NomeFases.MAQUINACAO &&
            f.Descricao == "Descricao")), Times.Once);
    }

    [Test(Description = "TFPSERV2 - Create deve falhar quando ja existe uma fase com o mesmo nome.")]
    public async Task Create_Should_ThrowBusinessConflictException_When_NomeAlreadyExists()
    {
        // ARRANGE
        var dto = new CreateFasesProducaoDto { Nome = NomeFases.EROSAO };
        _repository.Setup(r => r.GetByNomeAsync(NomeFases.EROSAO)).ReturnsAsync(BuildFase(id: 2, nome: NomeFases.EROSAO));

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<BusinessConflictException>()
            .WithMessage("Ja existe uma fase de producao com esse nome.");
    }

    [Test(Description = "TFPSERV3 - Update deve falhar quando a fase nao existe.")]
    public async Task Update_Should_ThrowKeyNotFoundException_When_FaseDoesNotExist()
    {
        // ARRANGE
        _repository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((FasesProducao?)null);

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(99, new UpdateFasesProducaoDto { Nome = NomeFases.MONTAGEM });

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "TFPSERV4 - Update deve falhar quando nao existem campos para atualizar.")]
    public async Task Update_Should_ThrowArgumentException_When_NoChangesAreProvided()
    {
        // ARRANGE
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(1, new UpdateFasesProducaoDto());

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Pelo menos um campo deve ser informado para atualizacao.");
    }

    [Test(Description = "TFPSERV5 - Update deve falhar quando o novo nome ja pertence a outra fase.")]
    public async Task Update_Should_ThrowBusinessConflictException_When_NomeAlreadyBelongsToAnotherFase()
    {
        // ARRANGE
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase(id: 1, nome: NomeFases.MAQUINACAO));
        _repository.Setup(r => r.GetByNomeAsync(NomeFases.MONTAGEM)).ReturnsAsync(BuildFase(id: 2, nome: NomeFases.MONTAGEM));

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(1, new UpdateFasesProducaoDto { Nome = NomeFases.MONTAGEM });
        // ASSERT
        await act.Should().ThrowAsync<BusinessConflictException>()
            .WithMessage("Ja existe uma fase de producao com esse nome.");
    }

    [Test(Description = "TFPSERV6 - Delete deve falhar quando a fase tem maquinas associadas.")]
    public async Task Delete_Should_ThrowBusinessConflictException_When_FaseHasAssociatedMaquinas()
    {
        // ARRANGE
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _repository.Setup(r => r.HasMaquinasAssociadasAsync(1)).ReturnsAsync(true);

        // ACT
        Func<Task> act = () => _sut.DeleteAsync(1);

        // ASSERT
        await act.Should().ThrowAsync<BusinessConflictException>()
            .WithMessage("Nao e possivel eliminar a fase de producao porque existem maquinas associadas.");
    }

    [Test(Description = "TFPSERV7 - GetAll deve devolver DTOs paginados quando o repositorio devolve dados.")]
    public async Task GetAll_Should_ReturnPagedDtos_When_RepositoryReturnsData()
    {
        // ARRANGE
        var fases = new List<FasesProducao>
        {
            BuildFase(1, NomeFases.MAQUINACAO),
            BuildFase(2, NomeFases.EROSAO)
        };

        _repository.Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(new PagedResult<FasesProducao>(fases, 2, 1, 10));

        // ACT
        var result = await _sut.GetAllAsync(1, 10);

        // ASSERT
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.Select(x => x.Nome).Should().Contain(new[] { NomeFases.MAQUINACAO, NomeFases.EROSAO });
    }
}
