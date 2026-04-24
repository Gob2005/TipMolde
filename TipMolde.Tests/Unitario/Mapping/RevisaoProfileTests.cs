using AutoMapper;
using FluentAssertions;
using TipMolde.Application.DTOs.RevisaoDTO;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Tests.Unitario.Mapping;

[TestFixture]
[Category("Unit")]
public class RevisaoProfileTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<RevisaoProfile>());
        _mapper = config.CreateMapper();
    }

    [Test(Description = "TREVMAP1 - Configuracao AutoMapper de RevisaoProfile deve ser valida.")]
    public void MappingConfiguration_Should_BeValid()
    {
        // ARRANGE
        var config = new MapperConfiguration(cfg => cfg.AddProfile<RevisaoProfile>());

        // ACT
        Action act = () => config.AssertConfigurationIsValid();

        // ASSERT
        act.Should().NotThrow();
    }

    [Test(Description = "TREVMAP2 - CreateRevisaoDTO deve mapear para Revisao com descricao trimada.")]
    public void CreateRevisaoDTO_Should_MapTo_Revisao()
    {
        // ARRANGE
        var source = new CreateRevisaoDTO
        {
            Projeto_id = 5,
            DescricaoAlteracoes = " Alteracao importante "
        };

        // ACT
        var result = _mapper.Map<Revisao>(source);

        // ASSERT
        result.Projeto_id.Should().Be(5);
        result.DescricaoAlteracoes.Should().Be("Alteracao importante");
    }

    [Test(Description = "TREVMAP3 - Entidade Revisao deve mapear para ResponseRevisaoDTO com dados de resposta.")]
    public void Revisao_Should_MapTo_ResponseRevisaoDTO()
    {
        // ARRANGE
        var source = new Revisao
        {
            Revisao_id = 10,
            NumRevisao = 3,
            DescricaoAlteracoes = "Rev 3",
            DataEnvioCliente = DateTime.UtcNow.AddDays(-1),
            Aprovado = false,
            DataResposta = DateTime.UtcNow,
            FeedbackTexto = "Necessita ajuste",
            FeedbackImagemPath = "/docs/rev3.png",
            Projeto_id = 7
        };

        // ACT
        var result = _mapper.Map<ResponseRevisaoDTO>(source);

        // ASSERT
        result.Revisao_id.Should().Be(10);
        result.NumRevisao.Should().Be(3);
        result.Aprovado.Should().BeFalse();
        result.FeedbackTexto.Should().Be("Necessita ajuste");
        result.Projeto_id.Should().Be(7);
    }
}
