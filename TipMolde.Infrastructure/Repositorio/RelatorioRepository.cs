using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Dtos.RelatorioDto;
using TipMolde.Application.Interface.Relatorios;
using TipMolde.Domain.Entities.Fichas;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa queries especializadas do modulo de relatorios.
    /// </summary>
    /// <remarks>
    /// Porque: os relatorios precisam de agregacoes transversais a comercio, desenho e producao.
    /// Devolver entidades EF diretamente aqui tornaria o formato dos documentos demasiado acoplado
    /// ao modelo de persistencia e aumentaria o risco de regressao.
    /// </remarks>
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly ApplicationDbContext _context;

        public RelatorioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Molde?> GetMoldeComEspecificacoesAsync(int moldeId) =>
            _context.Moldes
                .Include(m => m.Especificacoes)
                .FirstOrDefaultAsync(m => m.Molde_id == moldeId);

        public Task<FichaProducao?> GetFichaFltCompletaAsync(int fichaId) =>
            _context.FichasProducao
                .Include(f => f.EncomendaMolde)
                    .ThenInclude(em => em.Encomenda)
                        .ThenInclude(e => e.Cliente)
                .Include(f => f.EncomendaMolde)
                    .ThenInclude(em => em.Molde)
                        .ThenInclude(m => m.Especificacoes)
                .FirstOrDefaultAsync(f => f.FichaProducao_id == fichaId);
        public async Task<MoldeCicloVidaRelatorioDto?> ObterMoldeCicloVidaAsync(int moldeId)
        {
            var molde = await _context.Moldes
                .AsNoTracking()
                .Include(m => m.EncomendasMoldes)
                    .ThenInclude(em => em.Encomenda)
                        .ThenInclude(e => e.Cliente)
                .Include(m => m.Pecas)
                .FirstOrDefaultAsync(m => m.Molde_id == moldeId);

            if (molde is null)
                return null;

            var encomendaMoldeAtual = molde.EncomendasMoldes
                .OrderByDescending(em => em.DataEntregaPrevista)
                .FirstOrDefault();

            var pecaIds = molde.Pecas.Select(p => p.Peca_id).ToArray();

            var projetos = await _context.Projetos
                .AsNoTracking()
                .Where(p => p.Molde_id == moldeId)
                .Select(p => new MoldeProjetoResumoDto
                {
                    ProjetoId = p.Projeto_id,
                    NomeProjeto = p.NomeProjeto,
                    SoftwareUtilizado = p.SoftwareUtilizado,
                    TipoProjeto = p.TipoProjeto.ToString()
                })
                .ToListAsync();

            var projetoIds = projetos.Select(p => p.ProjetoId).ToArray();

            var totalRevisoes = projetoIds.Length == 0
                ? 0
                : await _context.Revisoes
                    .AsNoTracking()
                    .CountAsync(r => projetoIds.Contains(r.Projeto_id));

            var ultimaRevisaoEm = projetoIds.Length == 0
                ? null
                : await _context.Revisoes
                    .AsNoTracking()
                    .Where(r => projetoIds.Contains(r.Projeto_id))
                    .MaxAsync(r => (DateTime?)(r.DataResposta ?? r.DataEnvioCliente));

            var registos = pecaIds.Length == 0
                ? new List<RegistosProducao>()
                : await _context.RegistosProducao
                    .AsNoTracking()
                    .Where(r => pecaIds.Contains(r.Peca_id))
                    .ToListAsync();

            var fases = await _context.Fases_Producao
                .AsNoTracking()
                .ToDictionaryAsync(f => f.Fases_producao_id, f => f.Nome);

            var ultimosPorPecaEFase = registos
                .GroupBy(r => new { r.Peca_id, r.Fase_id })
                .Select(g => g.OrderByDescending(x => x.Data_hora).First())
                .ToList();

            int CountDistinctPiecesByPhase(NomeFases fase) =>
                ultimosPorPecaEFase
                    .Where(r => fases.TryGetValue(r.Fase_id, out var nome) &&
                                nome == fase &&
                                r.Estado_producao != EstadoProducao.PENDENTE)
                    .Select(r => r.Peca_id)
                    .Distinct()
                    .Count();

            var concluidas = ultimosPorPecaEFase
                .Where(r => fases.TryGetValue(r.Fase_id, out var nome) &&
                            nome == NomeFases.MONTAGEM &&
                            r.Estado_producao == EstadoProducao.CONCLUIDO)
                .Select(r => r.Peca_id)
                .Distinct()
                .Count();

            var emTrabalho = ultimosPorPecaEFase.Count(r =>
                r.Estado_producao is EstadoProducao.PREPARACAO or EstadoProducao.EM_CURSO or EstadoProducao.PAUSADO);

            var dto = new MoldeCicloVidaRelatorioDto
            {
                MoldeId = molde.Molde_id,
                NumeroMolde = molde.Numero,
                NumeroMoldeCliente = molde.NumeroMoldeCliente,
                NomeMolde = molde.Nome,
                DescricaoMolde = molde.Descricao,
                NumeroCavidades = molde.Numero_cavidades,
                TipoPedido = molde.TipoPedido,
                ClienteNome = encomendaMoldeAtual?.Encomenda?.Cliente?.Nome,
                NumeroEncomendaCliente = encomendaMoldeAtual?.Encomenda?.NumeroEncomendaCliente,
                NumeroProjetoCliente = encomendaMoldeAtual?.Encomenda?.NumeroProjetoCliente,
                NomeResponsavelCliente = encomendaMoldeAtual?.Encomenda?.NomeResponsavelCliente,
                DataRegistoEncomenda = encomendaMoldeAtual?.Encomenda?.DataRegisto,
                DataEntregaPrevista = encomendaMoldeAtual?.DataEntregaPrevista,
                TotalPecas = molde.Pecas.Count,
                MaterialPendente = molde.Pecas.Count(p => !p.MaterialRecebido),
                TotalProjetos = projetos.Count,
                TotalRevisoes = totalRevisoes,
                UltimaRevisaoEm = ultimaRevisaoEm,
                Maquinacao = CountDistinctPiecesByPhase(NomeFases.MAQUINACAO),
                Erosao = CountDistinctPiecesByPhase(NomeFases.EROSAO),
                Montagem = CountDistinctPiecesByPhase(NomeFases.MONTAGEM),
                EmTrabalho = emTrabalho,
                Concluidas = concluidas,
                PercentagemConclusao = molde.Pecas.Count == 0
                    ? 0
                    : Math.Round((decimal)concluidas / molde.Pecas.Count * 100m, 2),
                Projetos = projetos,
                Fases =
                [
                    new MoldeFaseResumoDto { NomeFase = NomeFases.MAQUINACAO.ToString(), PecasComMovimento = CountDistinctPiecesByPhase(NomeFases.MAQUINACAO) },
                    new MoldeFaseResumoDto { NomeFase = NomeFases.EROSAO.ToString(), PecasComMovimento = CountDistinctPiecesByPhase(NomeFases.EROSAO) },
                    new MoldeFaseResumoDto { NomeFase = NomeFases.MONTAGEM.ToString(), PecasComMovimento = CountDistinctPiecesByPhase(NomeFases.MONTAGEM) }
                ]
            };

            return dto;
        }

        /// <summary>
        /// Obtem o contexto base usado no preenchimento das fichas exportadas.
        /// </summary>
        /// <remarks>
        /// Esta query devolve apenas o shape necessario ao documento para evitar acoplamento
        /// entre a geracao do Excel e o modelo de persistencia completo.
        /// </remarks>
        /// <param name="fichaId">Identificador interno da ficha de producao.</param>
        /// <returns>Read-model base da ficha ou nulo quando a ficha nao existe.</returns>
        public Task<FichaRelatorioBaseDto?> ObterFichaRelatorioBaseAsync(int fichaId)
        {
            return _context.FichasProducao
                .AsNoTracking()
                .Where(f => f.FichaProducao_id == fichaId)
                .Select(f => new FichaRelatorioBaseDto
                {
                    FichaId = f.FichaProducao_id,
                    Tipo = f.Tipo,
                    MoldeNumero = f.EncomendaMolde!.Molde!.Numero,
                    MoldeNome = f.EncomendaMolde.Molde.Nome,
                    NumeroMoldeCliente = f.EncomendaMolde.Molde.NumeroMoldeCliente,
                    ImagemCapaPath = f.EncomendaMolde.Molde.ImagemCapaPath,
                    NumeroCavidades = f.EncomendaMolde.Molde.Numero_cavidades,
                    TipoPedido = f.EncomendaMolde.Molde.TipoPedido,
                    ClienteNome = f.EncomendaMolde.Encomenda!.Cliente!.Nome,
                    NomeServicoCliente = f.EncomendaMolde.Encomenda.NomeServicoCliente,
                    NumeroProjetoCliente = f.EncomendaMolde.Encomenda.NumeroProjetoCliente,
                    NomeResponsavelCliente = f.EncomendaMolde.Encomenda.NomeResponsavelCliente,
                    DataEntregaPrevista = f.EncomendaMolde.DataEntregaPrevista,
                    MaterialInjecao = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.MaterialInjecao : null,
                    Contracao = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.Contracao : null,
                    TipoInjecao = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.TipoInjecao : null,
                    AcabamentoPeca = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.AcabamentoPeca : null,
                    MaterialMacho = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.MaterialMacho : null,
                    MaterialCavidade = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.MaterialCavidade : null,
                    MaterialMovimentos = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.MaterialMovimentos : null,
                    SistemaInjecao = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.SistemaInjecao : null,
                    Cor = f.EncomendaMolde.Molde.Especificacoes != null ? f.EncomendaMolde.Molde.Especificacoes.Cor : null,
                    LadoFixo = f.EncomendaMolde.Molde.Especificacoes != null && f.EncomendaMolde.Molde.Especificacoes.LadoFixo,
                    LadoMovel = f.EncomendaMolde.Molde.Especificacoes != null && f.EncomendaMolde.Molde.Especificacoes.LadoMovel
                })
                .FirstOrDefaultAsync();
        }

    }
}
