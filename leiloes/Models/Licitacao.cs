using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace leiloes.Models
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
        public int leilao_IdLeilao { get; set; }

        [StringLength(9)]
        public string user_Nif { get; set; }

        public DateTime dataLicitacao { get; set; }

        // Relações de chave estrangeira
        [ForeignKey("leilao_IdLeilao")]
        public Leilao Leilao { get; set; }

        [ForeignKey("user_Nif")]
        public Utilizador Utilizador { get; set; }
    }
}
