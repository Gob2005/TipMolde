using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Comercio.IEncomendaMolde;
using TipMolde.Core.Interface.Fichas.IFichaProducao;
using TipMolde.Core.Models.Fichas;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Service
{
    public class FichaProducaoService : IFichaProducaoService
    {
        private readonly IFichaProducaoRepository _fichaRepository;
        private readonly IEncomendaMoldeRepository _encomendaMoldeRepository;
        private readonly ApplicationDbContext _context;

        public FichaProducaoService(
            IFichaProducaoRepository fichaRepository,
            IEncomendaMoldeRepository encomendaMoldeRepository,
            ApplicationDbContext context)
        {
            _fichaRepository = fichaRepository;
            _encomendaMoldeRepository = encomendaMoldeRepository;
            _context = context;
        }

        public Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId) =>
            _fichaRepository.GetByEncomendaMoldeIdAsync(encomendaMoldeId);

        public Task<FichaProducao?> GetByIdWithHeaderAsync(int id) =>
            _fichaRepository.GetByIdWithHeaderAsync(id);

        public Task<FichaProducao?> GetFLTByIdAsync(int id) =>
            _fichaRepository.GetFLTByIdAsync(id);

        public async Task<FichaProducao> CreateAsync(FichaProducao ficha)
        {
            var link = await _encomendaMoldeRepository.GetByIdAsync(ficha.EncomendaMolde_id);
            if (link == null)
                throw new KeyNotFoundException($"EncomendaMolde com ID {ficha.EncomendaMolde_id} nao encontrado.");

            ficha.DataGeracao = DateTime.UtcNow;
            await _fichaRepository.AddAsync(ficha);
            return ficha;
        }

        public async Task<RegistoOcorrencia> AddOcorrenciaAsync(int fichaId, RegistoOcorrencia ocorrencia)
        {
            var ficha = await _fichaRepository.GetByIdAsync(fichaId);
            if (ficha == null) throw new KeyNotFoundException("Ficha nao encontrada.");
            if (ficha.Tipo != TipoFicha.FOP) throw new ArgumentException("Ocorrencias apenas em ficha FOP.");

            ocorrencia.FichaProducao_id = fichaId;
            ocorrencia.DataOcorrencia = DateTime.UtcNow.Date;

            await _context.RegistosOcorrencia.AddAsync(ocorrencia);
            await _context.SaveChangesAsync();
            return ocorrencia;
        }

        public async Task<RegistoMelhoriaAlteracao> AddMelhoriaAlteracaoAsync(int fichaId, RegistoMelhoriaAlteracao registo)
        {
            var ficha = await _fichaRepository.GetByIdAsync(fichaId);
            if (ficha == null) throw new KeyNotFoundException("Ficha nao encontrada.");
            if (ficha.Tipo != TipoFicha.FRM && ficha.Tipo != TipoFicha.FRA)
                throw new ArgumentException("Registo de melhoria/alteracao apenas em ficha FRM/FRA.");

            registo.FichaProducao_id = fichaId;
            registo.DataRegisto = DateTime.UtcNow.Date;

            await _context.RegistosMelhoriaAlteracao.AddAsync(registo);
            await _context.SaveChangesAsync();
            return registo;
        }

        public async Task<RegistoEnsaio> UpsertEnsaioAsync(int fichaId, RegistoEnsaio ensaio)
        {
            var ficha = await _fichaRepository.GetByIdAsync(fichaId);
            if (ficha == null) throw new KeyNotFoundException("Ficha nao encontrada.");
            if (ficha.Tipo != TipoFicha.FRE) throw new ArgumentException("Registo de ensaio apenas em ficha FRE.");

            var existing = await _context.RegistosEnsaio.FirstOrDefaultAsync(x => x.FichaProducao_id == fichaId);

            if (existing == null)
            {
                ensaio.FichaProducao_id = fichaId;
                ensaio.DataEnsaio = DateTime.UtcNow.Date;
                await _context.RegistosEnsaio.AddAsync(ensaio);
                await _context.SaveChangesAsync();
                return ensaio;
            }

            existing.LocalEnsaio = ensaio.LocalEnsaio;
            existing.AguasCavidade = ensaio.AguasCavidade;
            existing.AguasMacho = ensaio.AguasMacho;
            existing.AguasMovimentos = ensaio.AguasMovimentos;
            existing.ResumoTexto = ensaio.ResumoTexto;
            existing.Maquina_id = ensaio.Maquina_id;
            existing.Responsavel_id = ensaio.Responsavel_id;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _fichaRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"FichaProducao com ID {id} nao encontrada.");

            await _fichaRepository.DeleteAsync(id);
        }
    }
}
