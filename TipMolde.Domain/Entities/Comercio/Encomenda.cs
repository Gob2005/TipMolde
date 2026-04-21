using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Comercio
{
    /// <summary>
    /// Representa um pedido de produção de um cliente.
    /// Pode conter múltiplos moldes através da relação N:M com EncomendaMolde.
    /// </summary>
    /// <remarks>
    /// O estado segue máquina de estados definida em ValidarTransicaoEstado (EncomendaService).
    /// Estados terminais (Concluída, Cancelada) não permitem transições posteriores.
    /// </remarks>
    public class Encomenda
    {
        public int Encomenda_id { get; set; }

        /// <summary>
        /// Número de referência da encomenda no sistema do cliente.
        /// Único para evitar duplicações (indexado).
        /// </summary>
        public required string NumeroEncomendaCliente { get; set; }

        /// <summary>
        /// Número do projeto associado no cliente (se aplicável).
        /// Usado para rastreabilidade documental.
        /// </summary>
        public string? NumeroProjetoCliente { get; set; }

        /// <summary>
        /// Nome do serviço no sistema do cliente (se aplicável).
        /// Usado para rastreabilidade documental.
        /// </summary>
        public string? NomeServicoCliente { get; set; }

        /// <summary>
        /// Nome do responsável do cliente para associado a encomenda (se aplicável).
        /// Usado para rastreabilidade documental.
        /// </summary>
        public string? NomeResponsavelCliente { get; set; }

        /// <summary>
        /// Data de registo da encomenda no sistema.
        /// Definida automaticamente em CreateAsync.
        /// </summary>
        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Estado atual da encomenda.
        /// Progressão controlada por ValidarTransicaoEstado para evitar transições inválidas.
        /// </summary>
        public EstadoEncomenda Estado { get; set; } = EstadoEncomenda.CONFIRMADA;

        public int Cliente_id { get; set; }
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Moldes desta encomenda (relação N:M).
        /// Permite uma encomenda ter múltiplos moldes com prazos diferentes.
        /// </summary>
        public ICollection<EncomendaMolde> EncomendasMoldes { get; set; } = new List<EncomendaMolde>();
    }
}