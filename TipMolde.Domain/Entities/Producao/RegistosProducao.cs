using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Producao
{
    /// <summary>
    /// Regista cada transição de estado de uma peça numa fase de produção.
    /// Estrutura imutável que implementa Event Sourcing simplificado.
    /// </summary>
    /// <remarks>
    /// Cada transição de estado (Preparação -> Em Curso -> Concluído) gera um novo registo.
    /// A duração de cada operação é calculada pela diferença temporal entre registos.
    /// Requisitos ISO 9001 (8.5.2): rastreabilidade de operador, máquina e timestamp.
    /// </remarks>
    public class RegistosProducao
    {
        public int Registo_Producao_id { get; set; }

        /// <summary>
        /// Estado alcançado nesta transição.
        /// Validado contra máquina de estados em RegistosProducaoService.
        /// </summary>
        public EstadoProducao Estado_producao { get; set; } = EstadoProducao.PENDENTE;

        /// <summary>
        /// Timestamp UTC da transição.
        /// Definido automaticamente no serviço (não pelo cliente).
        /// Usado para calcular tempo de ciclo entre estados.
        /// </summary>
        public DateTime Data_hora { get; set; }

        public int Fase_id { get; set; }
        public FasesProducao? Fase { get; set; }

        /// <summary>
        /// Operador responsável pela transição.
        /// Requisito ISO 9001 para rastreabilidade.
        /// </summary>
        public int Operador_id { get; set; }
        public User? Operador { get; set; }

        public int Peca_id { get; set; }
        public Peca? Peca { get; set; }

        /// <summary>
        /// Máquina utilizada (opcional).
        /// Obrigatório para estados PREPARACAO e EM_CURSO (validado no serviço).
        /// Usado para atualizar EstadoMaquina automaticamente.
        /// </summary>
        public int? Maquina_id { get; set; }
        public Maquina? Maquina { get; set; }
    }
}