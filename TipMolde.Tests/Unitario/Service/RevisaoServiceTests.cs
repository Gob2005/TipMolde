using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.Dtos.RevisaoDto;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Application.Interface.Desenho.IRevisao;
using TipMolde.Application.Mappings;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class RevisaoServiceTests
{
    private Mock<IRevisaoRepository> _revisaoRepository = null!;
    private Mock<IProjetoRepository> _projetoRepository = null!;
    private Mock<ILogger<RevisaoService>> _logger = null!;
    private IMapper _mapper = null!;
    private RevisaoService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _revisaoRepository = new Mock<IRevisaoRepository>();
        _projetoRepository = new Mock<IProjetoRepository>();
        _logger = new Mock<ILogger<RevisaoService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<RevisaoProfile>());
        _mapper = config.CreateMapper();

        _sut = new RevisaoService(
            _revisaoRepository.Object,
            _projetoRepository.Object,
            _mapper,
            _logger.Object);
    }

    [Test(Description = "TREVSRV1 - Create deve falhar quando o projeto referenciado nao existe.")]
    public async Task CreateAsync_Should_ThrowKeyNotFoundException_When_ProjetoDoesNotExist()
    {
        // ARRANGE
        var dto = new CreateRevisaoDto
        {
            Projeto_id = 50,
            DescricaoAlteracoes = "Nova alteracao"
        };

        _projetoRepository.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Projeto?)null);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*50*");
    }

    [Test(Description = "TREVSRV2 - Create deve persistir revisao e devolver DTO quando os dados sao validos.")]
    public async Task CreateAsync_Should_PersistRevisaoAndReturnResponse_When_DataIsValid()
    {
        // ARRANGE
        var dto = new CreateRevisaoDto
        {
            Projeto_id = 5,
            DescricaoAlteracoes = " Ajuste ao desenho "
        };

        _projetoRepository.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new Projeto
            {
                Projeto_id = 5,
                NomeProjeto = "Projeto 5",
                SoftwareUtilizado = "NX",
                CaminhoPastaServidor = @"\\srv\proj5"
            });

        _revisaoRepository.Setup(r => r.AddWithGeneratedNumeroAsync(It.IsAny<Revisao>()))
            .Callback<Revisao>(revisao =>
            {
                revisao.Revisao_id = 11;
                revisao.NumRevisao = 4;
            })
            .ReturnsAsync((Revisao revisao) => revisao);

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.Revisao_id.Should().Be(11);
        result.NumRevisao.Should().Be(4);
        result.DescricaoAlteracoes.Should().Be("Ajuste ao desenho");
        result.Projeto_id.Should().Be(5);
    }

    [Test(Description = "TREVSRV3 - UpdateRespostaCliente deve falhar quando a revisao ja tem resposta registada.")]
    public async Task UpdateRespostaClienteAsync_Should_ThrowBusinessConflictException_When_ResponseAlreadyExists()
    {
        // ARRANGE
        _revisaoRepository.Setup(r => r.GetByIdAsync(8))
            .ReturnsAsync(new Revisao
            {
                Revisao_id = 8,
                Projeto_id = 3,
                NumRevisao = 2,
                DescricaoAlteracoes = "Rev 2",
                Aprovado = true,
                DataResposta = DateTime.UtcNow.AddMinutes(-5)
            });

        var dto = new UpdateRespostaRevisaoDto
        {
            Aprovado = false,
            FeedbackTexto = "Novo feedback"
        };

        // ACT
        Func<Task> act = () => _sut.UpdateRespostaClienteAsync(8, dto);

        // ASSERT
        await act.Should().ThrowAsync<BusinessConflictException>();
    }

    [Test(Description = "TREVSRV4 - UpdateRespostaCliente deve falhar quando a revisao e rejeitada sem fundamentacao.")]
    public async Task UpdateRespostaClienteAsync_Should_ThrowArgumentException_When_RejectedWithoutFeedback()
    {
        // ARRANGE
        _revisaoRepository.Setup(r => r.GetByIdAsync(9))
            .ReturnsAsync(new Revisao
            {
                Revisao_id = 9,
                Projeto_id = 3,
                NumRevisao = 3,
                DescricaoAlteracoes = "Rev 3"
            });

        var dto = new UpdateRespostaRevisaoDto
        {
            Aprovado = false,
            FeedbackTexto = "   ",
            FeedbackImagemPath = null
        };

        // ACT
        Func<Task> act = () => _sut.UpdateRespostaClienteAsync(9, dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*FeedbackTexto ou FeedbackImagemPath*");
    }

    [Test(Description = "TREVSRV5 - UpdateRespostaCliente deve persistir a primeira resposta valida do cliente.")]
    public async Task UpdateRespostaClienteAsync_Should_UpdateRevisao_When_FirstResponseIsValid()
    {
        // ARRANGE
        _revisaoRepository.Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(new Revisao
            {
                Revisao_id = 10,
                Projeto_id = 4,
                NumRevisao = 1,
                DescricaoAlteracoes = "Rev 1"
            });

        var dto = new UpdateRespostaRevisaoDto
        {
            Aprovado = false,
            FeedbackTexto = " Necessita corrigir tolerancias ",
            FeedbackImagemPath = " /anexos/rev10.png "
        };

        // ACT
        await _sut.UpdateRespostaClienteAsync(10, dto);

        // ASSERT
        _revisaoRepository.Verify(r => r.UpdateAsync(It.Is<Revisao>(x =>
            x.Revisao_id == 10 &&
            x.Aprovado == false &&
            x.DataResposta.HasValue &&
            x.FeedbackTexto == "Necessita corrigir tolerancias" &&
            x.FeedbackImagemPath == "/anexos/rev10.png")), Times.Once);
    }
}
