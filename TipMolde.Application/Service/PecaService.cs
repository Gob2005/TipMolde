using System.Globalization;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TipMolde.Application.Dtos.PecaDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso da feature Peca.
    /// </summary>
    /// <remarks>
    /// Centraliza validacoes de negocio, atualizacao parcial, importacao CSV e orquestracao funcional
    /// da criacao, consulta, edicao e remocao de pecas associadas a um molde.
    /// </remarks>
    public class PecaService : IPecaService
    {
        private static readonly string[] CsvHeader =
        [
            "N PECA",
            "DESIGNACAO",
            "QTD",
            "REF",
            "MATERIAL",
            "TRAT TERMICO",
            "MASS",
            "OBS"
        ];

        private readonly IPecaRepository _pecaRepository;
        private readonly IMoldeRepository _moldeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PecaService> _logger;

        /// <summary>
        /// Construtor de PecaService.
        /// </summary>
        /// <param name="pecaRepository">Repositorio do agregado Peca.</param>
        /// <param name="moldeRepository">Repositorio usado para validar o molde associado.</param>
        /// <param name="mapper">Mapper para conversao entre Dtos e entidade.</param>
        /// <param name="logger">Logger para rastreabilidade das operacoes criticas.</param>
        public PecaService(
            IPecaRepository pecaRepository,
            IMoldeRepository moldeRepository,
            IMapper mapper,
            ILogger<PecaService> logger)
        {
            _pecaRepository = pecaRepository;
            _moldeRepository = moldeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Lista pecas de forma paginada.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com Dtos de peca.</returns>
        public async Task<PagedResult<ResponsePecaDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var result = await _pecaRepository.GetAllAsync(page, pageSize);
            var items = _mapper.Map<IEnumerable<ResponsePecaDto>>(result.Items);
            return new PagedResult<ResponsePecaDto>(items, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        /// <summary>
        /// Obtem uma peca por identificador.
        /// </summary>
        /// <param name="id">Identificador interno da peca.</param>
        /// <returns>DTO da peca quando encontrada; nulo caso contrario.</returns>
        public async Task<ResponsePecaDto?> GetByIdAsync(int id)
        {
            var peca = await _pecaRepository.GetByIdAsync(id);
            return peca == null ? null : _mapper.Map<ResponsePecaDto>(peca);
        }

        /// <summary>
        /// Lista pecas associadas a um molde.
        /// </summary>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com Dtos de peca.</returns>
        public async Task<PagedResult<ResponsePecaDto>> GetByMoldeIdAsync(int moldeId, int page = 1, int pageSize = 10)
        {
            var result = await _pecaRepository.GetByMoldeIdAsync(moldeId, page, pageSize);
            var items = _mapper.Map<IEnumerable<ResponsePecaDto>>(result.Items);
            return new PagedResult<ResponsePecaDto>(items, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        /// <summary>
        /// Obtem uma peca pela designacao dentro de um molde.
        /// </summary>
        /// <param name="designacao">Designacao funcional da peca.</param>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <returns>DTO da peca quando encontrada; nulo caso contrario.</returns>
        public async Task<ResponsePecaDto?> GetByDesignacaoAsync(string designacao, int moldeId)
        {
            var designacaoNormalizada = designacao.Trim();
            var peca = await _pecaRepository.GetByDesignacaoAsync(designacaoNormalizada, moldeId);
            return peca == null ? null : _mapper.Map<ResponsePecaDto>(peca);
        }

        /// <summary>
        /// Cria uma nova peca.
        /// </summary>
        /// <remarks>
        /// Fluxo critico:
        /// 1. Valida molde existente.
        /// 2. Valida designacao obrigatoria.
        /// 3. Garante unicidade por NumeroPeca quando esse identificador existe.
        /// 4. Usa a designacao como fallback de unicidade quando NumeroPeca nao e informado.
        /// 5. Persiste a peca e devolve o DTO estavel da feature.
        /// </remarks>
        /// <param name="dto">Dados de criacao da peca.</param>
        /// <returns>DTO da peca criada.</returns>
        public async Task<ResponsePecaDto> CreateAsync(CreatePecaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Designacao))
                throw new ArgumentException("Designacao e obrigatoria.");

            if (dto.Quantidade < 1)
                throw new ArgumentException("Quantidade deve ser maior ou igual a 1.");

            var molde = await _moldeRepository.GetByIdAsync(dto.Molde_id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {dto.Molde_id} nao encontrado.");

            var numeroPecaNormalizado = MappingProfileExtensions.NormalizeOptionalString(dto.NumeroPeca);
            var designacaoNormalizada = dto.Designacao.Trim();

            await ValidateUniquePecaAsync(dto.Molde_id, numeroPecaNormalizado, designacaoNormalizada, null);

            var peca = _mapper.Map<Peca>(dto);
            peca.NumeroPeca = numeroPecaNormalizado;
            peca.Designacao = designacaoNormalizada;
            peca.Quantidade = dto.Quantidade;

            await _pecaRepository.AddAsync(peca);

            _logger.LogInformation(
                "Peca {PecaId} criada com sucesso no molde {MoldeId}",
                peca.Peca_id,
                dto.Molde_id);

            return _mapper.Map<ResponsePecaDto>(peca);
        }

        /// <summary>
        /// Atualiza parcialmente uma peca existente.
        /// </summary>
        /// <remarks>
        /// Campos nao enviados no DTO devem manter o valor atual da entidade.
        /// </remarks>
        /// <param name="id">Identificador da peca a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        public async Task UpdateAsync(int id, UpdatePecaDto dto)
        {
            var existente = await _pecaRepository.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Peca com ID {id} nao encontrada.");

            if (!HasAnyChanges(dto))
                throw new ArgumentException("Pelo menos um campo deve ser informado para atualizacao.");

            var numeroPecaFuturo = ResolveFutureValue(dto.NumeroPeca, existente.NumeroPeca);
            var designacaoFutura = ResolveFutureRequiredValue(dto.Designacao, existente.Designacao);

            if (dto.Quantidade.HasValue && dto.Quantidade.Value < 1)
                throw new ArgumentException("Quantidade deve ser maior ou igual a 1.");

            await ValidateUniquePecaAsync(existente.Molde_id, numeroPecaFuturo, designacaoFutura, id);

            _mapper.Map(dto, existente);
            existente.NumeroPeca = numeroPecaFuturo;
            existente.Designacao = designacaoFutura;

            await _pecaRepository.UpdateAsync(existente);

            _logger.LogInformation("Peca {PecaId} atualizada com sucesso", id);
        }

        /// <summary>
        /// Importa pecas a partir de um ficheiro CSV da lista de materiais.
        /// </summary>
        /// <remarks>
        /// Fluxo critico:
        /// 1. Valida molde existente e estrutura do ficheiro.
        /// 2. Le a linha-resumo do molde.
        /// 3. Agrupa linhas por NumeroPeca.
        /// 4. Consolida quantidades quando os restantes campos coincidem.
        /// 5. Rejeita grupos contraditorios para o mesmo NumeroPeca.
        /// 6. Persiste as pecas consolidadas no molde indicado.
        /// </remarks>
        /// <param name="moldeId">Identificador do molde que recebe as pecas importadas.</param>
        /// <param name="csvStream">Stream do ficheiro CSV a processar.</param>
        /// <returns>Resumo da importacao com as pecas persistidas.</returns>
        public async Task<ImportPecasCsvResultDto> ImportarCsvAsync(int moldeId, Stream csvStream)
        {
            if (csvStream == null)
                throw new ArgumentNullException(nameof(csvStream));

            if (!csvStream.CanRead)
                throw new ArgumentException("O ficheiro CSV nao pode ser lido.");

            var molde = await _moldeRepository.GetByIdAsync(moldeId);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {moldeId} nao encontrado.");

            if (csvStream.CanSeek)
                csvStream.Seek(0, SeekOrigin.Begin);

            var parsedFile = await ParseCsvAsync(csvStream);
            var grupos = parsedFile.LinhasPeca
                .GroupBy(x => x.NumeroPeca, StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g.Min(x => x.NumeroLinha))
                .ToList();

            var resultado = new ImportPecasCsvResultDto
            {
                MoldeId = moldeId,
                ReferenciaMolde = parsedFile.ReferenciaMolde,
                MassaMolde = parsedFile.MassaMolde,
                TotalLinhasPecaLidas = parsedFile.LinhasPeca.Count
            };

            var pecasConsolidadas = new List<Peca>();
            var prioridadeAtual = 1;

            foreach (var grupo in grupos)
            {
                var linhasGrupo = grupo.OrderBy(x => x.NumeroLinha).ToList();
                ValidateCsvGroupConsistency(grupo.Key, linhasGrupo);

                var pecaConsolidada = BuildPecaFromCsvGroup(moldeId, prioridadeAtual, linhasGrupo);
                await ValidateUniquePecaAsync(moldeId, pecaConsolidada.NumeroPeca, pecaConsolidada.Designacao, null);

                pecasConsolidadas.Add(pecaConsolidada);
                prioridadeAtual++;
            }

            foreach (var pecaConsolidada in pecasConsolidadas)
            {
                await _pecaRepository.AddAsync(pecaConsolidada);
                resultado.PecasImportadas.Add(_mapper.Map<ResponsePecaDto>(pecaConsolidada));
                resultado.TotalQuantidadeConsolidada += pecaConsolidada.Quantidade;
            }

            resultado.TotalPecasConsolidadas = pecasConsolidadas.Count;

            _logger.LogInformation(
                "Importacao CSV de pecas concluida para Molde {MoldeId} com {TotalPecas} pecas consolidadas e {TotalQuantidade} unidades",
                moldeId,
                resultado.TotalPecasConsolidadas,
                resultado.TotalQuantidadeConsolidada);

            return resultado;
        }

        /// <summary>
        /// Remove uma peca existente.
        /// </summary>
        /// <param name="id">Identificador da peca a remover.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        public async Task DeleteAsync(int id)
        {
            var peca = await _pecaRepository.GetByIdAsync(id);
            if (peca == null)
                throw new KeyNotFoundException($"Peca com ID {id} nao encontrada.");

            await _pecaRepository.DeleteAsync(id);

            _logger.LogInformation("Peca {PecaId} removida com sucesso", id);
        }

        /// <summary>
        /// Verifica se o pedido de update contem pelo menos uma alteracao funcional.
        /// </summary>
        /// <param name="dto">DTO de atualizacao parcial.</param>
        /// <returns>True quando existe pelo menos um campo preenchido; false caso contrario.</returns>
        private static bool HasAnyChanges(UpdatePecaDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.NumeroPeca)
                || !string.IsNullOrWhiteSpace(dto.Designacao)
                || dto.Prioridade.HasValue
                || dto.Quantidade.HasValue
                || dto.Referencia != null
                || dto.MaterialDesignacao != null
                || dto.TratamentoTermico != null
                || dto.Massa != null
                || dto.Observacao != null
                || dto.MaterialRecebido.HasValue;
        }

        private async Task ValidateUniquePecaAsync(int moldeId, string? numeroPeca, string designacao, int? currentPecaId)
        {
            if (!string.IsNullOrWhiteSpace(numeroPeca))
            {
                var duplicadoPorNumero = await _pecaRepository.GetByNumeroPecaAsync(numeroPeca, moldeId);
                if (duplicadoPorNumero != null && duplicadoPorNumero.Peca_id != currentPecaId)
                    throw new ArgumentException($"Ja existe uma peca com o NumeroPeca '{numeroPeca}' neste molde.");

                return;
            }

            var duplicadoPorDesignacao = await _pecaRepository.GetByDesignacaoAsync(designacao, moldeId);
            if (duplicadoPorDesignacao != null && duplicadoPorDesignacao.Peca_id != currentPecaId)
                throw new ArgumentException("Ja existe uma peca com esta designacao neste molde.");
        }

        private static string? ResolveFutureValue(string? incomingValue, string? currentValue)
        {
            return incomingValue == null
                ? currentValue
                : MappingProfileExtensions.NormalizeOptionalString(incomingValue);
        }

        private static string ResolveFutureRequiredValue(string? incomingValue, string currentValue)
        {
            return string.IsNullOrWhiteSpace(incomingValue)
                ? currentValue
                : incomingValue.Trim();
        }

        private static Peca BuildPecaFromCsvGroup(int moldeId, int prioridade, IReadOnlyCollection<PecaCsvLinhaDto> linhasGrupo)
        {
            var primeiraLinha = linhasGrupo.OrderBy(x => x.NumeroLinha).First();

            return new Peca
            {
                NumeroPeca = primeiraLinha.NumeroPeca,
                Designacao = primeiraLinha.Designacao,
                Prioridade = prioridade,
                Quantidade = linhasGrupo.Sum(x => x.Quantidade),
                Referencia = primeiraLinha.Referencia,
                MaterialDesignacao = primeiraLinha.MaterialDesignacao,
                TratamentoTermico = primeiraLinha.TratamentoTermico,
                Massa = primeiraLinha.Massa,
                Observacao = primeiraLinha.Observacao,
                MaterialRecebido = false,
                Molde_id = moldeId
            };
        }

        private static void ValidateCsvGroupConsistency(string numeroPeca, IReadOnlyCollection<PecaCsvLinhaDto> linhasGrupo)
        {
            var primeiraLinha = linhasGrupo.OrderBy(x => x.NumeroLinha).First();

            foreach (var linha in linhasGrupo.Skip(1))
            {
                var camposConflitantes = GetConflictingFields(primeiraLinha, linha);
                if (camposConflitantes.Count == 0)
                    continue;

                var numerosLinha = string.Join(", ", linhasGrupo.Select(x => x.NumeroLinha));
                var campos = string.Join(", ", camposConflitantes);

                throw new ArgumentException(
                    $"O NumeroPeca '{numeroPeca}' aparece com dados contraditorios nas linhas {numerosLinha}. " +
                    $"Os campos {campos} devem ser iguais para consolidar a quantidade.");
            }
        }

        private static List<string> GetConflictingFields(PecaCsvLinhaDto referencia, PecaCsvLinhaDto atual)
        {
            var conflitos = new List<string>();

            if (!StringEquals(referencia.Designacao, atual.Designacao))
                conflitos.Add("Designacao");

            if (!StringEquals(referencia.Referencia, atual.Referencia))
                conflitos.Add("Referencia");

            if (!StringEquals(referencia.MaterialDesignacao, atual.MaterialDesignacao))
                conflitos.Add("Material");

            if (!StringEquals(referencia.TratamentoTermico, atual.TratamentoTermico))
                conflitos.Add("TratamentoTermico");

            if (!StringEquals(referencia.Massa, atual.Massa))
                conflitos.Add("Massa");

            if (!StringEquals(referencia.Observacao, atual.Observacao))
                conflitos.Add("Observacao");

            return conflitos;
        }

        private static bool StringEquals(string? left, string? right)
        {
            return string.Equals(
                MappingProfileExtensions.NormalizeOptionalString(left),
                MappingProfileExtensions.NormalizeOptionalString(right),
                StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<ParsedPecaCsvFile> ParseCsvAsync(Stream csvStream)
        {
            using var memoryStream = new MemoryStream();
            await csvStream.CopyToAsync(memoryStream);

            var lines = DecodeCsvLines(memoryStream.ToArray());
            var headerLine = lines.ElementAtOrDefault(0);
            if (string.IsNullOrWhiteSpace(headerLine))
                throw new ArgumentException("O ficheiro CSV nao contem cabecalho.");

            var headerColumns = SplitCsvLine(headerLine);
            ValidateCsvHeader(headerColumns);

            var metadataLine = lines.ElementAtOrDefault(1);
            if (metadataLine == null)
                throw new ArgumentException("O ficheiro CSV nao contem a linha-resumo do molde.");

            var metadataColumns = SplitCsvLine(metadataLine);
            ValidateColumnCount(metadataColumns, 2);
            ValidateMetadataRow(metadataColumns);

            var linhasPeca = new List<PecaCsvLinhaDto>();

            for (var index = 2; index < lines.Length; index++)
            {
                var line = lines[index];
                var lineNumber = index + 1;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = SplitCsvLine(line);
                ValidateColumnCount(columns, lineNumber);
                linhasPeca.Add(ParsePieceRow(columns, lineNumber));
            }

            if (linhasPeca.Count == 0)
                throw new ArgumentException("O ficheiro CSV nao contem linhas de pecas para importar.");

            return new ParsedPecaCsvFile(
                MappingProfileExtensions.NormalizeOptionalString(metadataColumns[3]),
                MappingProfileExtensions.NormalizeOptionalString(metadataColumns[6]),
                linhasPeca);
        }

        private static void ValidateCsvHeader(IReadOnlyList<string> headerColumns)
        {
            ValidateColumnCount(headerColumns, 1);

            for (var i = 0; i < CsvHeader.Length; i++)
            {
                if (string.Equals(CanonicalizeCsvToken(headerColumns[i]), CsvHeader[i], StringComparison.Ordinal))
                    continue;

                throw new ArgumentException(
                    $"Cabecalho CSV invalido. Esperado '{CsvHeader[i]}' na coluna {i + 1}, mas recebido '{headerColumns[i]}'.");
            }
        }

        private static void ValidateColumnCount(IReadOnlyList<string> columns, int lineNumber)
        {
            if (columns.Count == CsvHeader.Length)
                return;

            throw new ArgumentException(
                $"A linha {lineNumber} do CSV contem {columns.Count} colunas. Eram esperadas {CsvHeader.Length} colunas.");
        }

        private static void ValidateMetadataRow(IReadOnlyList<string> metadataColumns)
        {
            var numeroPeca = MappingProfileExtensions.NormalizeOptionalString(metadataColumns[0]);
            var designacao = MappingProfileExtensions.NormalizeOptionalString(metadataColumns[1]);
            var referencia = MappingProfileExtensions.NormalizeOptionalString(metadataColumns[3]);

            if (!string.IsNullOrWhiteSpace(numeroPeca) || !string.IsNullOrWhiteSpace(designacao))
                throw new ArgumentException("A linha 2 do CSV deve representar o resumo do molde e nao uma peca.");

            if (!string.Equals(CanonicalizeCsvToken(referencia), "MOLDE", StringComparison.Ordinal))
                throw new ArgumentException("A linha 2 do CSV deve ter 'Molde' na coluna Ref.");
        }

        private static PecaCsvLinhaDto ParsePieceRow(IReadOnlyList<string> columns, int lineNumber)
        {
            var numeroPeca = MappingProfileExtensions.NormalizeOptionalString(columns[0]);
            if (string.IsNullOrWhiteSpace(numeroPeca))
                throw new ArgumentException($"A linha {lineNumber} nao tem NumeroPeca preenchido.");

            var designacao = MappingProfileExtensions.NormalizeOptionalString(columns[1]);
            if (string.IsNullOrWhiteSpace(designacao))
                throw new ArgumentException($"A linha {lineNumber} nao tem Designacao preenchida.");

            var quantidadeRaw = MappingProfileExtensions.NormalizeOptionalString(columns[2]);
            if (!int.TryParse(quantidadeRaw, out var quantidade) || quantidade < 1)
                throw new ArgumentException($"A linha {lineNumber} tem uma quantidade invalida.");

            return new PecaCsvLinhaDto
            {
                NumeroLinha = lineNumber,
                NumeroPeca = numeroPeca,
                Designacao = designacao,
                Quantidade = quantidade,
                Referencia = MappingProfileExtensions.NormalizeOptionalString(columns[3]),
                MaterialDesignacao = MappingProfileExtensions.NormalizeOptionalString(columns[4]),
                TratamentoTermico = MappingProfileExtensions.NormalizeOptionalString(columns[5]),
                Massa = MappingProfileExtensions.NormalizeOptionalString(columns[6]),
                Observacao = MappingProfileExtensions.NormalizeOptionalString(columns[7])
            };
        }

        private static IReadOnlyList<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var insideQuotes = false;

            foreach (var character in line)
            {
                if (character == '"')
                {
                    insideQuotes = !insideQuotes;
                    continue;
                }

                if (character == ';' && !insideQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(character);
            }

            result.Add(current.ToString());
            return result;
        }

        private static string[] DecodeCsvLines(byte[] csvBytes)
        {
            foreach (var encoding in GetCandidateEncodings())
            {
                var text = encoding.GetString(csvBytes);
                var lines = SplitLines(text);
                var headerLine = lines.ElementAtOrDefault(0);

                if (!string.IsNullOrWhiteSpace(headerLine) && IsExpectedCsvHeader(SplitCsvLine(headerLine)))
                    return lines;
            }

            return SplitLines(Encoding.UTF8.GetString(csvBytes));
        }

        private static IEnumerable<Encoding> GetCandidateEncodings()
        {
            yield return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false);
            yield return Encoding.GetEncoding(1252);
        }

        private static string[] SplitLines(string content)
        {
            return content
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n')
                .Split('\n');
        }

        private static bool IsExpectedCsvHeader(IReadOnlyList<string> headerColumns)
        {
            if (headerColumns.Count != CsvHeader.Length)
                return false;

            for (var i = 0; i < CsvHeader.Length; i++)
            {
                if (!string.Equals(CanonicalizeCsvToken(headerColumns[i]), CsvHeader[i], StringComparison.Ordinal))
                    return false;
            }

            return true;
        }

        private static string CanonicalizeCsvToken(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = value.Replace("\uFEFF", string.Empty, StringComparison.Ordinal)
                .Trim()
                .Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder();

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                    continue;

                if (character <= '\u007F' && char.IsLetterOrDigit(character))
                {
                    builder.Append(char.ToUpperInvariant(character));
                    continue;
                }

                if (char.IsWhiteSpace(character) || character is '.' or '_' or '-')
                    builder.Append(' ');
            }

            return string.Join(' ', builder.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        private sealed record ParsedPecaCsvFile(
            string? ReferenciaMolde,
            string? MassaMolde,
            List<PecaCsvLinhaDto> LinhasPeca);
    }
}
