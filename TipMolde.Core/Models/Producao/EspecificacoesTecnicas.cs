using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.Core.Models.Producao
{
    /// <summary>
    /// Armazena características técnicas detalhadas de um molde.
    /// Separada da entidade Molde para evitar poluição da tabela principal
    /// e permitir futuras extensões sem impacto na estrutura base.
    /// </summary>
    /// <remarks>
    /// Relação 1:1 com Molde (FK = PK).
    /// Todos os campos são opcionais porque nem todos os moldes têm todas as especificações.
    /// </remarks>
    public class EspecificacoesTecnicas
    {
        /// <summary>
        /// Chave primária que é também chave estrangeira para Molde.
        /// Garante relação 1:1 ao nível da base de dados.
        /// </summary>
        [Key]
        public int Molde_id { get; set; }

        public Molde? Molde { get; set; }

        // Dimensões físicas (em mm)
        public decimal? Largura { get; set; }
        public decimal? Comprimento { get; set; }
        public decimal? Altura { get; set; }
        public decimal? PesoEstimado { get; set; } // em kg

        // Características de injeção
        public string? TipoInjecao { get; set; }      // Ex: "Hot Runner", "Cold Runner"
        public string? SistemaInjecao { get; set; }   // Ex: "Canal Quente"
        public decimal? Contracao { get; set; }       // Percentagem de contração do material

        // Acabamento e materiais
        public string? AcabamentoPeca { get; set; }   // Ex: "Polido", "Texturado"
        public CorMolde? Cor { get; set; }              // Ex: "MONOCOLOR", "BICOLOR", "OUTRO"

        /// <summary>
        /// Materiais dos componentes do molde.
        /// Importantes para rastreabilidade e manutenção preventiva.
        /// </summary>
        public string? MaterialMacho { get; set; }      // Ex: "AISI P20"
        public string? MaterialCavidade { get; set; }   // Ex: "H13"
        public string? MaterialMovimentos { get; set; } // Ex: "AISI 420"
        public string? MaterialInjecao { get; set; }    // Ex: "ABS", "Polipropileno"

        /// <summary>
        /// Flags para identificar onde estão instalados os componentes.
        /// Necessário para operações de manutenção e desmontagem.
        /// </summary>
        public bool LadoFixo { get; set; }
        public bool LadoMovel { get; set; }
    }
}