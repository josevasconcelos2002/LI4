using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace leiloes
{
    [Table("Licitacao")]
    public class Licitacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdLicitacao { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }

        [Required]
        public int LeilaoIdLeilao { get; set; }

        [StringLength(9)]
        public string UserNif { get; set; }

        // Relações de chave estrangeira
        [ForeignKey("LeilaoIdLeilao")]
        public Leilao Leilao { get; set; }

        [ForeignKey("UserNif")]
        public Utilizador Utilizador { get; set; }
    }
}
