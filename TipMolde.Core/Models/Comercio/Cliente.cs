namespace TipMolde.Core.Models.Comercio
{
    /// <summary>
    /// Representa uma empresa cliente da TipMolde.
    /// Entidade principal do módulo comercial.
    /// </summary>
    /// <remarks>
    /// NIF e Sigla são únicos e indexados para pesquisas rápidas.
    /// CreatedAt permite análise de aquisição de clientes ao longo do tempo.
    /// </remarks>
    public class Cliente
    {
        public int Cliente_id { get; set; }

        public required string Nome { get; set; }

        /// <summary>
        /// Número de Identificação Fiscal (único em Portugal).
        /// Validado por índice único na base de dados.
        /// </summary>
        public required string NIF { get; set; }

        /// <summary>
        /// Sigla identificadora do cliente (ex: "BMW", "VW").
        /// Usada em relatórios e documentos para economizar espaço.
        /// </summary>
        public required string Sigla { get; set; }

        public string? Pais { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }

        /// <summary>
        /// Data de criação do registo.
        /// Útil para análise de crescimento da carteira de clientes.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Encomendas associadas a este cliente.
        /// Navegação 1:N.
        /// </summary>
        public ICollection<Encomenda> Encomendas { get; set; } = new List<Encomenda>();
    }
}