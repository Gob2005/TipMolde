namespace TipMolde.Domain.Entities.Comercio
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
        /// <summary>
        /// Identificador unico do cliente.
        /// </summary>
        public int Cliente_id { get; set; }

        /// <summary>
        /// Nome comercial do cliente.
        /// </summary>
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

        /// <summary>
        /// Pais de localizacao principal do cliente.
        /// </summary>
        public string? Pais { get; set; }

        /// <summary>
        /// Endereco de email de contacto comercial do cliente.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Contacto telefonico principal do cliente.
        /// </summary>
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
