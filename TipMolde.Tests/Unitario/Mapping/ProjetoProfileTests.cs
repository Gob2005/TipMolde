using AutoMapper;
using FluentAssertions;
using TipMolde.Application.DTOs.ProjetoDTO;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Mapping;

[TestFixture]
[Category("Unit")]
public class ProjetoProfileTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProjetoProfile>();
            cfg.AddProfile<RevisaoProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Test(Description = "TPROJMAP1 - Configuracao AutoMapper de ProjetoProfile deve ser valida.")]
    public void MappingConfiguration_Should_BeValid()
    {
        // ARRANGE
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProjetoProfile>();
            cfg.AddProfile<RevisaoProfile>();
        });

        // ACT
        Action act = () => config.AssertConfigurationIsValid();

        // ASSERT
        act.Should().NotThrow();
    }

    [Test(Description = "TPROJMAP2 - Entidade Projeto deve mapear para ResponseProjetoDTO com caminho persistido.")]
    public void Projeto_Should_MapTo_ResponseProjetoDTO()
    {
        // ARRANGE
        var source = new Projeto
        {
            Projeto_id = 5,
            NomeProjeto = "Projeto 5",
            SoftwareUtilizado = "SolidWorks",
            TipoProjeto = TipoProjeto.PROJETO_3D,
            CaminhoPastaServidor = @"\\srv\projetos\proj-5",
            Molde_id = 10
        };

        // ACT
        var result = _mapper.Map<ResponseProjetoDTO>(source);

        // ASSERT
        result.Projeto_id.Should().Be(5);
        result.NomeProjeto.Should().Be("Projeto 5");
        result.CaminhoPastaServidor.Should().Be(@"\\srv\projetos\proj-5");
        result.Molde_id.Should().Be(10);
    }

    [Test(Description = "TPROJMAP3 - Entidade Projeto deve mapear para detalhe com revisoes ordenadas por numero decrescente.")]
    public void Projeto_Should_MapTo_ResponseProjetoWithRevisoesDTO()
    {
        // ARRANGE
        var source = new Projeto
        {
            Projeto_id = 6,
            NomeProjeto = "Projeto 6",
            SoftwareUtilizado = "AutoCAD",
            TipoProjeto = TipoProjeto.PROJETO_2D,
            CaminhoPastaServidor = @"\\srv\projetos\proj-6",
            Molde_id = 11,
            Revisoes = new List<Revisao>
            {
                new() { Revisao_id = 1, NumRevisao = 1, DescricaoAlteracoes = "Primeira", Projeto_id = 6 },
                new() { Revisao_id = 2, NumRevisao = 3, DescricaoAlteracoes = "Terceira", Projeto_id = 6 },
                new() { Revisao_id = 3, NumRevisao = 2, DescricaoAlteracoes = "Segunda", Projeto_id = 6 }
            }
        };

        // ACT
        var result = _mapper.Map<ResponseProjetoWithRevisoesDTO>(source);

        // ASSERT
        result.Revisoes.Select(r => r.NumRevisao).Should().Equal(3, 2, 1);
    }

    [Test(Description = "TPROJMAP4 - UpdateProjetoDTO deve atualizar apenas campos preenchidos e preservar os restantes.")]
    public void UpdateProjetoDTO_Should_MapOnlyProvidedFields_When_MappingToExistingProjeto()
    {
        // ARRANGE
        var source = new UpdateProjetoDTO
        {
            NomeProjeto = "Projeto Atualizado"
        };

        var destination = new Projeto
        {
            Projeto_id = 9,
            NomeProjeto = "Projeto Antigo",
            SoftwareUtilizado = "NX",
            TipoProjeto = TipoProjeto.PROJETO_3D,
            CaminhoPastaServidor = @"\\srv\projetos\origem",
            Molde_id = 20
        };

        // ACT
        _mapper.Map(source, destination);

        // ASSERT
        destination.NomeProjeto.Should().Be("Projeto Atualizado");
        destination.SoftwareUtilizado.Should().Be("NX");
        destination.TipoProjeto.Should().Be(TipoProjeto.PROJETO_3D);
        destination.CaminhoPastaServidor.Should().Be(@"\\srv\projetos\origem");
    }
}
