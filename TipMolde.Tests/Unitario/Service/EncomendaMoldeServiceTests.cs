using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.DTOs.EncomendaMoldeDTO;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface.Comercio.IEncomenda;
using TipMolde.Application.Interface.Comercio.IEncomendaMolde;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class EncomendaMoldeServiceTests
{
    private Mock<IEncomendaMoldeRepository> _repo = null!;
    private Mock<IEncomendaRepository> _encomendaRepo = null!;
    private Mock<IMoldeRepository> _moldeRepo = null!;
    private Mock<IMapper> _mapper = null!;
    private Mock<ILogger<EncomendaMoldeService>> _logger = null!;
    private EncomendaMoldeService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IEncomendaMoldeRepository>();
        _encomendaRepo = new Mock<IEncomendaRepository>();
        _moldeRepo = new Mock<IMoldeRepository>();
        _mapper = new Mock<IMapper>();
        _logger = new Mock<ILogger<EncomendaMoldeService>>();

        _sut = new EncomendaMoldeService(_repo.Object, _encomendaRepo.Object, _moldeRepo.Object, _mapper.Object, _logger.Object);
    }

    [Test(Description = "TENCMSRV1 - Create deve falhar com conflito quando ja existe o par Encomenda-Molde.")]
    public async Task CreateAsync_Should_ThrowBusinessConflictException_When_AssociationAlreadyExists()
    {
        // ARRANGE
        var dto = new CreateEncomendaMoldeDTO
        {
            Encomenda_id = 1,
            Molde_id = 2,
            Quantidade = 10,
            Prioridade = 1,
            DataEntregaPrevista = DateTime.UtcNow.AddDays(7)
        };

        _encomendaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Encomenda { Encomenda_id = 1, NumeroEncomendaCliente = "ENC-1" });
        _moldeRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Molde { Molde_id = 2, Numero = "M-1" });
        _repo.Setup(r => r.ExistsAssociationAsync(1, 2, null)).ReturnsAsync(true);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<BusinessConflictException>();
    }

    [Test(Description = "TENCMSRV2 - Update deve falhar quando nenhum campo de patch e enviado.")]
    public async Task UpdateAsync_Should_ThrowArgumentException_When_NoFieldsProvided()
    {
        // ARRANGE
        var dto = new UpdateEncomendaMoldeDTO();
        _repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new EncomendaMolde
        {
            EncomendaMolde_id = 10,
            Encomenda_id = 1,
            Molde_id = 1,
            Quantidade = 5,
            Prioridade = 1,
            DataEntregaPrevista = DateTime.UtcNow
        });

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(10, dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
