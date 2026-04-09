using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TipMolde.Core.Interface.Fichas.IFichaDocumento;
using TipMolde.Core.Models.Fichas;

namespace TipMolde.Infrastructure.Service
{
    public class FichaDocumentoService : IFichaDocumentoService
    {
        private readonly IFichaDocumentoRepository _fdRepository;
        private readonly IConfiguration _config;

        public FichaDocumentoService(IFichaDocumentoRepository fdRepository, IConfiguration config)
        {
            _fdRepository = fdRepository;
            _config = config;
        }

        public async Task<FichaDocumento> GuardarGeradoAsync(int fichaId, byte[] content, string fileName, string tipoFicheiro, int userId, string origem = "SISTEMA")
        {
            if (!await _fdRepository.FichaExisteAsync(fichaId))
                throw new KeyNotFoundException($"Ficha {fichaId} nao existe.");

            var root = _config["Storage:FichasRootPath"] ?? @"C:\Users\HP\Documents\TipMolde\Storage\Fichas";
            var dir = Path.Combine(root, fichaId.ToString());
            Directory.CreateDirectory(dir);

            var versao = await _fdRepository.GetProximaVersaoAsync(fichaId);
            await _fdRepository.DesativarVersoesAtivasAsync(fichaId);

            var nomeFinal = $"{Path.GetFileNameWithoutExtension(fileName)}_v{versao}{Path.GetExtension(fileName)}";
            var path = Path.Combine(dir, nomeFinal);

            await File.WriteAllBytesAsync(path, content);

            var doc = new FichaDocumento
            {
                FichaProducao_id = fichaId,
                Versao = versao,
                Origem = origem,
                NomeFicheiro = nomeFinal,
                TipoFicheiro = tipoFicheiro,
                CaminhoFicheiro = path,
                HashSha256 = ComputeSha256(content),
                CriadoPor_user_id = userId,
                Ativo = true
            };

            await _fdRepository.AddAsync(doc);
            return doc;
        }

        public async Task<FichaDocumento> UploadAsync(int fichaId, IFormFile file, int userId)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            return await GuardarGeradoAsync(
                fichaId,
                ms.ToArray(),
                file.FileName,
                string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                userId,
                "UPLOAD");
        }

        public Task<IEnumerable<FichaDocumento>> ListarAsync(int fichaId) => _fdRepository.GetByFichaIdAsync(fichaId);

        public async Task<(byte[] Content, string FileName, string TipoFicheiro)> DownloadAsync(int documentoId)
        {
            var doc = await _fdRepository.GetByIdAsync(documentoId)
                ?? throw new KeyNotFoundException("Documento nao encontrado.");

            if (!File.Exists(doc.CaminhoFicheiro))
                throw new FileNotFoundException($"Ficheiro nao existe no disco: {doc.CaminhoFicheiro}");

            var bytes = await File.ReadAllBytesAsync(doc.CaminhoFicheiro);
            return (bytes, doc.NomeFicheiro, doc.TipoFicheiro);
        }

        private static string ComputeSha256(byte[] bytes)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash); // 64 chars
        }
    }
}
