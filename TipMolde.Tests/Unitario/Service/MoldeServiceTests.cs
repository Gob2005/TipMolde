using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.DTOs.MoldeDTO;
using TipMolde.Application.Interface.Comercio.IEncomenda;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Mappings;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class MoldeServiceTests
{
    private Mock<IMoldeRepository> _moldeRepository = null!;
    private Mock<IEncomendaRepository> _encomendaRepository = null!;
    private Mock<ILogger<MoldeService>> _logger = null!;
    private IMapper _mapper = null!;
    private MoldeService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _moldeRepository = new Mock<IMoldeRepository>();
        _encomendaRepository = new Mock<IEncomendaRepository>();
        _logger = new Mock<ILogger<MoldeService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MoldeProfile>());
        _mapper = config.CreateMapper();

        _sut = new MoldeService(
            _moldeRepository.Object,
            _encomendaRepository.Object,
            _mapper,
            _logger.Object);
    }

    private static CreateMoldeDTO BuildCreateDto(string numero = " MOL-001 ")
    {
        return new CreateMoldeDTO
        {
            Numero = numero,
            NumeroMoldeCliente = "CLI-001",
            Nome = "Molde Base",
            ImagemCapaPath = "imagens/molde.png",
            Descricao = "Descricao do molde",
            Numero_cavidades = 4,
            TipoPedido = TipoPedido.NOVO_MOLDE,
            Largura = 10,
            Comprimento = 20,
            Altura = 30,
            PesoEstimado = 40,
            TipoInjecao = "Hot Runner",
            SistemaInjecao = "Canal Quente",
            Contracao = 1.25m,
            AcabamentoPeca = "Polido",
            Cor = CorMolde.MONOCOLOR,
            MaterialMacho = "P20",
            MaterialCavidade = "H13",
            MaterialMovimentos = "420",
            MaterialInjecao = "ABS",
            EncomendaId = 7,
            Quantidade = 10,
            Prioridade = 1,
            DataEntregaPrevista = new DateTime(2026, 5, 10)
        };
    }

    private static Molde BuildMolde(int id = 1, string numero = "MOL-001", TipoPedido tipoPedido = TipoPedido.NOVO_MOLDE)
    {
        return new Molde
        {
            Molde_id = id,
            Numero = numero,
            NumeroMoldeCliente = "CLI-001",
            Nome = "Molde Atual",
            Descricao = "Descricao atual",
            Numero_cavidades = 4,
            TipoPedido = tipoPedido,
            ImagemCapaPath = "capa.png",
            Especificacoes = new EspecificacoesTecnicas
            {
                Molde_id = id,
                Largura = 10,
                Comprimento = 20,
                Altura = 30,
                PesoEstimado = 40,
                TipoInjecao = "Hot Runner",
                SistemaInjecao = "Canal Quente",
                Contracao = 1.10m,
                AcabamentoPeca = "Mate",
                Cor = CorMolde.MONOCOLOR,
                MaterialMacho = "P20",
                MaterialCavidade = "H13",
                MaterialMovimentos = "420",
                MaterialInjecao = "ABS"
            }
        };
    }

    [Test(Description = "TMOLDSRV1 - Create deve falhar quando numero do molde e vazio.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_NumeroIsBlank()
    {
        // ARRANGE
        var dto = BuildCreateDto("   ");

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Numero do molde e obrigatorio*");
    }

    [Test(Description = "TMOLDSRV2 - Create deve falhar quando ja existe um molde com o mesmo numero.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_NumeroAlreadyExists()
    {
        // ARRANGE
        var dto = BuildCreateDto(" MOL-100 ");
        _moldeRepository.Setup(r => r.GetByNumeroAsync("MOL-100"))
            .ReturnsAsync(BuildMolde(id: 10, numero: "MOL-100"));

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Ja existe um molde com este numero*");
    }

    [Test(Description = "TMOLDSRV3 - Create deve falhar quando a encomenda referenciada nao existe.")]
    public async Task CreateAsync_Should_ThrowKeyNotFoundException_When_EncomendaDoesNotExist()
    {
        // ARRANGE
        var dto = BuildCreateDto();
        _moldeRepository.Setup(r => r.GetByNumeroAsync("MOL-001")).ReturnsAsync((Molde?)null);
        _encomendaRepository.Setup(r => r.GetByIdAsync(dto.EncomendaId)).ReturnsAsync((Encomenda?)null);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{dto.EncomendaId}*");
    }

    [Test(Description = "TMOLDSRV4 - Create deve persistir molde, especificacoes e associacao quando dados sao validos.")]
    public async Task CreateAsync_Should_PersistMoldeSpecsAndLink_When_DataIsValid()
    {
        // ARRANGE
        var dto = BuildCreateDto();
        _moldeRepository.Setup(r => r.GetByNumeroAsync("MOL-001")).ReturnsAsync((Molde?)null);
        _encomendaRepository.Setup(r => r.GetByIdAsync(dto.EncomendaId))
            .ReturnsAsync(new Encomenda
            {
                Encomenda_id = dto.EncomendaId,
                NumeroEncomendaCliente = "ENC-007"
            });

        _moldeRepository
            .Setup(r => r.AddMoldeWithSpecsAndLinkAsync(It.IsAny<Molde>(), It.IsAny<EspecificacoesTecnicas>(), It.IsAny<EncomendaMolde>()))
            .Callback<Molde, EspecificacoesTecnicas, EncomendaMolde>((molde, specs, link) =>
            {
                molde.Molde_id = 25;
                specs.Molde_id = 25;
                link.Molde_id = 25;
            })
            .Returns(Task.CompletedTask);

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.MoldeId.Should().Be(25);
        result.Numero.Should().Be("MOL-001");
        result.NumeroMoldeCliente.Should().Be("CLI-001");

        _moldeRepository.Verify(r => r.AddMoldeWithSpecsAndLinkAsync(
            It.Is<Molde>(m =>
                m.Numero == "MOL-001" &&
                m.NumeroMoldeCliente == "CLI-001" &&
                m.Numero_cavidades == 4),
            It.Is<EspecificacoesTecnicas>(s =>
                s.Largura == 10 &&
                s.MaterialInjecao == "ABS"),
            It.Is<EncomendaMolde>(l =>
                l.Encomenda_id == dto.EncomendaId &&
                l.Quantidade == dto.Quantidade &&
                l.Prioridade == dto.Prioridade)),
            Times.Once);
    }

    [Test(Description = "TMOLDSRV5 - Update deve falhar quando nenhum campo e enviado.")]
    public async Task UpdateAsync_Should_ThrowArgumentException_When_NoFieldsProvided()
    {
        // ARRANGE
        var existente = BuildMolde(id: 11);
        var dto = new UpdateMoldeDTO();

        _moldeRepository.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(existente);

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(11, dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Pelo menos um campo*");
    }

    [Test(Description = "TMOLDSRV6 - Update deve preservar TipoPedido quando o campo nao e enviado.")]
    public async Task UpdateAsync_Should_PreserveTipoPedido_When_FieldIsNotSent()
    {
        // ARRANGE
        var existente = BuildMolde(id: 12, numero: "MOL-012", tipoPedido: TipoPedido.ALTERACAO);
        var dto = new UpdateMoldeDTO
        {
            Nome = "Novo Nome",
            MaterialInjecao = "PP"
        };

        _moldeRepository.Setup(r => r.GetByIdAsync(12)).ReturnsAsync(existente);

        // ACT
        await _sut.UpdateAsync(12, dto);

        // ASSERT
        _moldeRepository.Verify(r => r.UpdateAsync(It.Is<Molde>(m =>
            m.Molde_id == 12 &&
            m.Nome == "Novo Nome" &&
            m.TipoPedido == TipoPedido.ALTERACAO &&
            m.Especificacoes != null &&
            m.Especificacoes.MaterialInjecao == "PP")), Times.Once);
    }

    [Test(Description = "TMOLDSRV7 - Delete deve remover molde quando o registo existe.")]
    public async Task DeleteAsync_Should_Delete_When_MoldeExists()
    {
        // ARRANGE
        _moldeRepository.Setup(r => r.GetByIdAsync(13)).ReturnsAsync(BuildMolde(id: 13));

        // ACT
        await _sut.DeleteAsync(13);

        // ASSERT
        _moldeRepository.Verify(r => r.DeleteAsync(13), Times.Once);
    }
}
