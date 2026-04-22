using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Producao
{
    /// <summary>
    /// Representa o agregado principal de um molde no dominio de producao.
    /// </summary>
    /// <remarks>
    /// O molde centraliza os dados de identificacao funcional, caracterizacao tecnica
    /// e relacoes com encomendas e pecas do processo produtivo.
    /// </remarks>
    public class Molde
    {
        /// <summary>
        /// Identificador interno do molde.
        /// </summary>
        public int Molde_id { get; set; }

        /// <summary>
        /// Numero funcional unico do molde.
        /// </summary>
        /// <remarks>
        /// Este numero e o identificador mais usado pelos utilizadores nas pesquisas
        /// e na comunicacao operacional entre equipas.
        /// </remarks>
        public required string Numero { get; set; }

        /// <summary>
        /// Referencia do molde no sistema do cliente.
        /// </summary>
        /// <remarks>
        /// Permite manter rastreabilidade entre o identificador interno da TipMolde
        /// e o codigo de negocio usado externamente pelo cliente.
        /// </remarks>
        public string? NumeroMoldeCliente { get; set; }

        /// <summary>
        /// Nome curto ou designacao funcional do molde.
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        /// Descricao funcional ou tecnica adicional do molde.
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Numero de cavidades do molde.
        /// </summary>
        /// <remarks>
        /// Este valor influencia a capacidade produtiva e e validado no contrato de entrada.
        /// </remarks>
        public int Numero_cavidades { get; set; }

        /// <summary>
        /// Tipo de pedido associado ao molde.
        /// </summary>
        /// <remarks>
        /// Distingue se o trabalho corresponde a um novo molde, reparacao ou alteracao,
        /// condicionando o contexto funcional do registo.
        /// </remarks>
        public TipoPedido TipoPedido { get; set; }

        /// <summary>
        /// Caminho da imagem de capa do molde.
        /// </summary>
        /// <remarks>
        /// A imagem e usada em fichas, consultas e relatorios do agregado.
        /// </remarks>
        public string? ImagemCapaPath { get; set; }

        /// <summary>
        /// Especificacoes tecnicas detalhadas do molde.
        /// </summary>
        /// <remarks>
        /// Relacao 1:1 usada para separar os atributos tecnicos da tabela principal do molde.
        /// </remarks>
        public EspecificacoesTecnicas? Especificacoes { get; set; }

        /// <summary>
        /// Pecas que pertencem ao molde.
        /// </summary>
        /// <remarks>
        /// Cada peca pode seguir o seu proprio ciclo dentro das fases de producao.
        /// </remarks>
        public ICollection<Peca> Pecas { get; set; } = new List<Peca>();

        /// <summary>
        /// Associacoes entre o molde e as encomendas.
        /// </summary>
        /// <remarks>
        /// A relacao e materializada por EncomendaMolde para guardar dados de negocio
        /// da associacao, como quantidade, prioridade e prazo previsto.
        /// </remarks>
        public ICollection<EncomendaMolde> EncomendasMoldes { get; set; } = new List<EncomendaMolde>();
    }
}
