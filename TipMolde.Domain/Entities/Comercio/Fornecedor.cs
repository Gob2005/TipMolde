namespace TipMolde.Domain.Entities.Comercio
{
    /// <summary>
    /// Representa um fornecedor de matéria-prima ou componentes.
    /// Usado no módulo de compras e aprovisionamento.
    /// </summary>
    /// <remarks>
    /// NIF é único e indexado (validação de duplicados em FornecedorService).
    /// </remarks>
    public class Fornecedor
    {
        public int Fornecedor_id { get; set; }

        public required string Nome { get; set; }

        /// <summary>
        /// Número de Identificação Fiscal do fornecedor.
        /// Único para evitar duplicação de cadastros.
        /// </summary>
        public required string NIF { get; set; }

        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Morada { get; set; }
    }
}