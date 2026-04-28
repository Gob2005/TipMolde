using TipMolde.Application.Interface;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Centraliza as regras de paginacao usadas pelos services da aplicacao.
    /// </summary>
    /// <remarks>
    /// Esta classe mantem a decisao funcional de limites de pagina fora dos repositories,
    /// deixando a camada de persistencia apenas responsavel por executar queries ja normalizadas.
    /// </remarks>
    internal static class PaginationDefaults
    {
        /// <summary>
        /// Numero minimo permitido para uma pagina solicitada.
        /// </summary>
        public const int DefaultPage = 1;

        /// <summary>
        /// Quantidade minima funcional de itens por pagina.
        /// </summary>
        public const int MinPageSize = 10;

        /// <summary>
        /// Quantidade maxima funcional de itens por pagina.
        /// </summary>
        public const int MaxPageSize = 200;

        /// <summary>
        /// Normaliza os parametros de paginacao recebidos pela camada de aplicacao.
        /// </summary>
        /// <param name="page">Numero de pagina solicitado pelo consumidor.</param>
        /// <param name="pageSize">Quantidade de itens por pagina solicitada pelo consumidor.</param>
        /// <returns>Tuplo com pagina e tamanho de pagina dentro dos limites funcionais.</returns>
        public static (int Page, int PageSize) Normalize(int page, int pageSize)
        {
            return (NormalizePage(page), NormalizePageSize(pageSize));
        }

        /// <summary>
        /// Garante que a pagina solicitada nunca fica abaixo da primeira pagina.
        /// </summary>
        /// <param name="page">Numero de pagina solicitado pelo consumidor.</param>
        /// <returns>Pagina normalizada para consulta.</returns>
        public static int NormalizePage(int page)
        {
            return Math.Max(page, DefaultPage);
        }

        /// <summary>
        /// Garante que o tamanho da pagina respeita os limites funcionais definidos.
        /// </summary>
        /// <param name="pageSize">Quantidade de itens por pagina solicitada pelo consumidor.</param>
        /// <returns>Tamanho de pagina limitado entre o minimo e o maximo aceites.</returns>
        public static int NormalizePageSize(int pageSize)
        {
            return Math.Clamp(pageSize, MinPageSize, MaxPageSize);
        }

        /// <summary>
        /// Cria uma pagina vazia com metadados de paginacao normalizados.
        /// </summary>
        /// <typeparam name="T">Tipo dos itens da pagina.</typeparam>
        /// <param name="page">Numero de pagina solicitado pelo consumidor.</param>
        /// <param name="pageSize">Quantidade de itens por pagina solicitada pelo consumidor.</param>
        /// <returns>Resultado paginado vazio com pagina e tamanho normalizados.</returns>
        public static PagedResult<T> EmptyPage<T>(int page, int pageSize)
        {
            var (normalizedPage, normalizedPageSize) = Normalize(page, pageSize);

            return new PagedResult<T>(
                Enumerable.Empty<T>(),
                0,
                normalizedPage,
                normalizedPageSize);
        }
    }
}
