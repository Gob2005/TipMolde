using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.DTOs.EncomendaDTO;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Application.Interface.Comercio.IEncomenda;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
public class EncomendaServiceTests
{
    private Mock<IEncomendaRepository> _encomendaRepository = null!;
    private Mock<IClienteRepository> _clienteRepository = null!;
    private Mock<IMapper> _mapper = null!;
    private Mock<ILogger<EncomendaService>> _logger = null!;
    private EncomendaService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _encomendaRepository = new Mock<IEncomendaRepository>();
        _clienteRepository = new Mock<IClienteRepository>();
        _mapper = new Mock<IMapper>();
        _logger = new Mock<ILogger<EncomendaService>>();

        _mapper.Setup(m => m.Map<ResponseEncomendaDTO>(It.IsAny<Encomenda>()))
            .Returns((Encomenda e) => new ResponseEncomendaDTO
            {
                Encomenda_id = e.Encomenda_id,
                NumeroEncomendaCliente = e.NumeroEncomendaCliente,
                NumeroProjetoCliente = e.NumeroProjetoCliente,
                NomeServicoCliente = e.NomeServicoCliente,
                NomeResponsavelCliente = e.NomeResponsavelCliente,
                DataRegisto = e.DataRegisto,
                Estado = e.Estado,
                Cliente_id = e.Cliente_id
            });

        _mapper.Setup(m => m.Map<IEnumerable<ResponseEncomendaDTO>>(It.IsAny<IEnumerable<Encomenda>>()))
            .Returns((IEnumerable<Encomenda> list) => list.Select(e => new ResponseEncomendaDTO
            {
                Encomenda_id = e.Encomenda_id,
                NumeroEncomendaCliente = e.NumeroEncomendaCliente,
                NumeroProjetoCliente = e.NumeroProjetoCliente,
                NomeServicoCliente = e.NomeServicoCliente,
                NomeResponsavelCliente = e.NomeResponsavelCliente,
                DataRegisto = e.DataRegisto,
                Estado = e.Estado,
                Cliente_id = e.Cliente_id
            }).ToList());

        _sut = new EncomendaService(
            _encomendaRepository.Object,
            _clienteRepository.Object,
            _mapper.Object,
            _logger.Object);
    }

    private static Encomenda BuildEncomenda(int id = 1, string numero = "ENC-001", EstadoEncomenda estado = EstadoEncomenda.CONFIRMADA)
    {
        return new Encomenda
        {
            Encomenda_id = id,
            NumeroEncomendaCliente = numero,
            NumeroProjetoCliente = "PRJ-1",
            NomeServicoCliente = "Servico",
            NomeResponsavelCliente = "Maria",
            Cliente_id = 10,
            Estado = estado,
            DataRegisto = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc)
        };
    }

    [Test(Description = "TENCSRV1 - Create deve falhar quando numero de encomenda e vazio.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_NumeroIsBlank()
    {
        // ARRANGE
        var dto = new CreateEncomendaDTO
        {
            Cliente_id = 10,
            NumeroEncomendaCliente = "   "
        };

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*numero de encomenda*");
    }

    [Test(Description = "TENCSRV2 - Create deve falhar quando cliente nao existe.")]
    public async Task CreateAsync_Should_ThrowKeyNotFoundException_When_ClienteDoesNotExist()
    {
        // ARRANGE
        var dto = new CreateEncomendaDTO
        {
            Cliente_id = 10,
            NumeroEncomendaCliente = "ENC-001"
        };
        _clienteRepository.Setup(r => r.GetByIdAsync(dto.Cliente_id)).ReturnsAsync((Cliente?)null);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{dto.Cliente_id}*");
    }

    [Test(Description = "TENCSRV3 - Create deve falhar com conflito quando numero ja existe.")]
    public async Task CreateAsync_Should_ThrowBusinessConflictException_When_NumeroAlreadyExists()
    {
        // ARRANGE
        var dto = new CreateEncomendaDTO
        {
            Cliente_id = 10,
            NumeroEncomendaCliente = " ENC-100 "
        };

        _clienteRepository.Setup(r => r.GetByIdAsync(dto.Cliente_id)).ReturnsAsync(new Cliente
        {
            Cliente_id = dto.Cliente_id,
            Nome = "Cliente",
            NIF = "123456789",
            Sigla = "CLI"
        });
        _encomendaRepository.Setup(r => r.ExistsNumeroEncomendaClienteAsync("ENC-100", null)).ReturnsAsync(true);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<BusinessConflictException>()
            .WithMessage("*ENC-100*");
    }

    [Test(Description = "TENCSRV4 - Create deve normalizar numero e definir estado inicial confirmada.")]
    public async Task CreateAsync_Should_SetDefaultsAndPersist_When_DataIsValid()
    {
        // ARRANGE
        var dto = new CreateEncomendaDTO
        {
            Cliente_id = 10,
            NumeroEncomendaCliente = " ENC-200 ",
            NumeroProjetoCliente = "PRJ-200",
            NomeServicoCliente = "Servico",
            NomeResponsavelCliente = "Ana"
        };

        _clienteRepository.Setup(r => r.GetByIdAsync(dto.Cliente_id)).ReturnsAsync(new Cliente
        {
            Cliente_id = dto.Cliente_id,
            Nome = "Cliente",
            NIF = "123456789",
            Sigla = "CLI"
        });
        _encomendaRepository.Setup(r => r.ExistsNumeroEncomendaClienteAsync("ENC-200", null)).ReturnsAsync(false);
        _encomendaRepository.Setup(r => r.AddAsync(It.IsAny<Encomenda>()))
            .ReturnsAsync((Encomenda e) =>
            {
                e.Encomenda_id = 55;
                return e;
            });

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.Encomenda_id.Should().Be(55);
        result.NumeroEncomendaCliente.Should().Be("ENC-200");
        result.Estado.Should().Be(EstadoEncomenda.CONFIRMADA);
        _encomendaRepository.Verify(r => r.AddAsync(It.IsAny<Encomenda>()), Times.Once);
    }

    [Test(Description = "TENCSRV5 - Update deve falhar quando encomenda nao existe.")]
    public async Task UpdateAsync_Should_ThrowKeyNotFoundException_When_EncomendaDoesNotExist()
    {
        // ARRANGE
        var dto = new UpdateEncomendaDTO { NumeroEncomendaCliente = "ENC-500" };
        _encomendaRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Encomenda?)null);

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(99, dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "TENCSRV6 - Update deve falhar com conflito quando novo numero ja existe noutra encomenda.")]
    public async Task UpdateAsync_Should_ThrowBusinessConflictException_When_NewNumeroAlreadyExists()
    {
        // ARRANGE
        var existing = BuildEncomenda(id: 10, numero: "ENC-010");
        var dto = new UpdateEncomendaDTO { NumeroEncomendaCliente = " ENC-999 " };

        _encomendaRepository.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(existing);
        _encomendaRepository.Setup(r => r.ExistsNumeroEncomendaClienteAsync("ENC-999", 10)).ReturnsAsync(true);

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(10, dto);

        // ASSERT
        await act.Should().ThrowAsync<BusinessConflictException>();
    }

    [Test(Description = "TENCSRV7 - Update deve atualizar campos permitidos e manter valores nao informados.")]
    public async Task UpdateAsync_Should_UpdatePartialFields_When_DataIsValid()
    {
        // ARRANGE
        var existing = BuildEncomenda(id: 20, numero: "ENC-020");
        existing.NumeroProjetoCliente = "PRJ-OLD";
        existing.NomeServicoCliente = "Servico Antigo";
        existing.NomeResponsavelCliente = "Ana";

        var dto = new UpdateEncomendaDTO
        {
            NumeroEncomendaCliente = " ENC-021 ",
            NomeServicoCliente = "Servico Novo"
        };

        _encomendaRepository.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(existing);
        _encomendaRepository.Setup(r => r.ExistsNumeroEncomendaClienteAsync("ENC-021", 20)).ReturnsAsync(false);

        // ACT
        await _sut.UpdateAsync(20, dto);

        // ASSERT
        _encomendaRepository.Verify(r => r.UpdateAsync(It.Is<Encomenda>(e =>
            e.Encomenda_id == 20 &&
            e.NumeroEncomendaCliente == "ENC-021" &&
            e.NumeroProjetoCliente == "PRJ-OLD" &&
            e.NomeServicoCliente == "Servico Novo" &&
            e.NomeResponsavelCliente == "Ana")), Times.Once);
    }

    [Test(Description = "TENCSRV8 - UpdateEstado deve devolver erro quando encomenda nao existe.")]
    public async Task UpdateEstadoAsync_Should_ThrowKeyNotFoundException_When_EncomendaDoesNotExist()
    {
        // ARRANGE
        _encomendaRepository.Setup(r => r.GetByIdAsync(45)).ReturnsAsync((Encomenda?)null);

        // ACT
        Func<Task> act = () => _sut.UpdateEstadoAsync(45, new UpdateEstadoEncomendaDTO { Estado = EstadoEncomenda.EM_PRODUCAO });

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "TENCSRV9 - UpdateEstado deve falhar em transicao invalida.")]
    public async Task UpdateEstadoAsync_Should_ThrowArgumentException_When_TransitionIsInvalid()
    {
        // ARRANGE
        var encomenda = BuildEncomenda(id: 30, estado: EstadoEncomenda.CONCLUIDA);
        _encomendaRepository.Setup(r => r.GetByIdAsync(30)).ReturnsAsync(encomenda);

        // ACT
        Func<Task> act = () => _sut.UpdateEstadoAsync(30, new UpdateEstadoEncomendaDTO { Estado = EstadoEncomenda.EM_PRODUCAO });

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Transicao de estado invalida*");
    }

    [Test(Description = "TENCSRV10 - Delete deve apagar encomenda quando registo existe.")]
    public async Task DeleteAsync_Should_Delete_When_EncomendaExists()
    {
        // ARRANGE
        _encomendaRepository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(BuildEncomenda(id: 7));

        // ACT
        await _sut.DeleteAsync(7);

        // ASSERT
        _encomendaRepository.Verify(r => r.DeleteAsync(7), Times.Once);
    }

    [Test(Description = "TENCSRV11 - GetByNumero deve falhar quando numero e vazio.")]
    public async Task GetByNumeroEncomendaClienteAsync_Should_ThrowArgumentException_When_NumeroIsBlank()
    {
        // ARRANGE

        // ACT
        Func<Task> act = async () => await _sut.GetByNumeroEncomendaClienteAsync("   ");

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*obrigatorio*");
    }
}
