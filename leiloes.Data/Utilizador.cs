using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace leiloes.Data
{
    [Table("Utilizador")]
    public class Utilizador
    {
        [Key]
        [StringLength(9)]
        public string Nif { get; set; }

        [Required]
        [StringLength(45)]
        public string Nome { get; set; }

        [Required]
        [StringLength(45)]
        public string Username { get; set; }

        [Required]
        [StringLength(45)]
        public string Email { get; set; }

        [Required]
        [StringLength(45)]
        public string Password { get; set; }

        [Required]
        public int UserType { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Saldo { get; set; }
    }
}
