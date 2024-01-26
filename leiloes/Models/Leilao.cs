using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace leiloes.Models
{
    [Table("Leilao")]
    public class Leilao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdLeilao { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal LicitacaoAtual { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal PrecoMinLicitacao { get; set; }

        [StringLength(50)]
        public string? Estado { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        [StringLength(9)]
        public string CriadorId { get; set; }

        public int ProdutoId { get; set; }

        // Relações de chave estrangeira
        [ForeignKey("CriadorId")]
        public Utilizador? Criador { get; set; }

        [ForeignKey("ProdutoId")]
        public Produto? Produto { get; set; }
    }
}
