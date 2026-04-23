using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.DTOs.ProjetoDTO;
using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Mappings;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class ProjetoServiceTests
{
    private Mock<IProjetoRepository> _projetoRepository = null!;
    private Mock<IMoldeRepository> _moldeRepository = null!;
    private Mock<ILogger<ProjetoService>> _logger = null!;
    private IMapper _mapper = null!;
    private ProjetoService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _projetoRepository = new Mock<IProjetoRepository>();
        _moldeRepository = new Mock<IMoldeRepository>();
        _logger = new Mock<ILogger<ProjetoService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProjetoProfile>());
        _mapper = config.CreateMapper();

        _sut = new ProjetoService(
            _projetoRepository.Object,
            _moldeRepository.Object,
            _mapper,
            _logger.Object);
    }

    private static CreateProjetoDTO BuildCreateDto(string caminho = @" \\srv\projetos\molde-01 ")
    {
        return new CreateProjetoDTO
        {
            NomeProjeto = " Projeto Base ",
            SoftwareUtilizado = " SolidWorks ",
            TipoProjeto = TipoProjeto.PROJETO_3D,
            CaminhoPastaServidor = caminho,
            Molde_id = 7
        };
    }

    private static Projeto BuildProjeto(int id = 1, TipoProjeto tipoProjeto = TipoProjeto.PROJETO_3D)
    {
        return new Projeto
        {
            Projeto_id = id,
            NomeProjeto = "Projeto Atual",
            SoftwareUtilizado = "NX",
            TipoProjeto = tipoProjeto,
            CaminhoPastaServidor = @"\\srv\projetos\origem",
            Molde_id = 9
        };
    }

    [Test(Description = "TPROJSRV1 - Create deve falhar quando caminho da pasta e vazio.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_CaminhoPastaServidorIsBlank()
    {
        // ARRANGE
        var dto = BuildCreateDto("   ");

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Caminho da pasta no servidor e obrigatorio*");
    }

    [Test(Description = "TPROJSRV2 - Create deve falhar quando o molde referenciado nao existe.")]
    public async Task CreateAsync_Should_ThrowKeyNotFoundException_When_MoldeDoesNotExist()
    {
        // ARRANGE
        var dto = BuildCreateDto();
        _moldeRepository.Setup(r => r.GetByIdAsync(dto.Molde_id)).ReturnsAsync((Molde?)null);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{dto.Molde_id}*");
    }

    [Test(Description = "TPROJSRV3 - Create deve persistir projeto e devolver DTO com caminho do servidor.")]
    public async Task CreateAsync_Should_PersistProjetoAndReturnResponse_When_DataIsValid()
    {
        // ARRANGE
        var dto = BuildCreateDto();

        _moldeRepository.Setup(r => r.GetByIdAsync(dto.Molde_id))
            .ReturnsAsync(new Molde
            {
                Molde_id = dto.Molde_id,
                Numero = "MOL-007",
                Numero_cavidades = 4,
                TipoPedido = TipoPedido.NOVO_MOLDE
            });

        _projetoRepository.Setup(r => r.AddAsync(It.IsAny<Projeto>()))
            .Callback<Projeto>(projeto => projeto.Projeto_id = 25)
            .ReturnsAsync((Projeto projeto) => projeto);

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.Projeto_id.Should().Be(25);
        result.NomeProjeto.Should().Be("Projeto Base");
        result.SoftwareUtilizado.Should().Be("SolidWorks");
        result.CaminhoPastaServidor.Should().Be(@"\\srv\projetos\molde-01");
        result.TipoProjeto.Should().Be(TipoProjeto.PROJETO_3D);

        _projetoRepository.Verify(r => r.AddAsync(It.Is<Projeto>(p =>
            p.NomeProjeto == "Projeto Base" &&
            p.SoftwareUtilizado == "SolidWorks" &&
            p.CaminhoPastaServidor == @"\\srv\projetos\molde-01" &&
            p.TipoProjeto == TipoProjeto.PROJETO_3D &&
            p.Molde_id == dto.Molde_id)), Times.Once);
    }

    [Test(Description = "TPROJSRV4 - Update deve falhar quando nenhum campo e enviado.")]
    public async Task UpdateAsync_Should_ThrowArgumentException_When_NoFieldsProvided()
    {
        // ARRANGE
        var existente = BuildProjeto(id: 11);
        var dto = new UpdateProjetoDTO();

        _projetoRepository.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(existente);

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(11, dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Pelo menos um campo*");
    }

    [Test(Description = "TPROJSRV5 - Update deve preservar TipoProjeto quando o campo nao e enviado.")]
    public async Task UpdateAsync_Should_PreserveTipoProjeto_When_FieldIsNotSent()
    {
        // ARRANGE
        var existente = BuildProjeto(id: 12, tipoProjeto: TipoProjeto.PROJETO_3D);
        var dto = new UpdateProjetoDTO
        {
            NomeProjeto = "Projeto Atualizado"
        };

        _projetoRepository.Setup(r => r.GetByIdAsync(12)).ReturnsAsync(existente);

        // ACT
        await _sut.UpdateAsync(12, dto);

        // ASSERT
        _projetoRepository.Verify(r => r.UpdateAsync(It.Is<Projeto>(p =>
            p.Projeto_id == 12 &&
            p.NomeProjeto == "Projeto Atualizado" &&
            p.TipoProjeto == TipoProjeto.PROJETO_3D &&
            p.CaminhoPastaServidor == @"\\srv\projetos\origem")), Times.Once);
    }

    [Test(Description = "TPROJSRV6 - Delete deve remover projeto quando o registo existe.")]
    public async Task DeleteAsync_Should_Delete_When_ProjetoExists()
    {
        // ARRANGE
        _projetoRepository.Setup(r => r.GetByIdAsync(13)).ReturnsAsync(BuildProjeto(id: 13));

        // ACT
        await _sut.DeleteAsync(13);

        // ASSERT
        _projetoRepository.Verify(r => r.DeleteAsync(13), Times.Once);
    }
}
