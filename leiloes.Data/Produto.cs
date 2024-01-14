﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace leiloes.Data
{
    [Table("Produto")]
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProduto { get; set; }

        [Required]
        [StringLength(45)]
        public string Nome { get; set; }

        [Required]
        [StringLength(255)]
        public string Descricao { get; set; }

        [StringLength(255)]
        public string Imagem { get; set; }

        [Required]
        public int NumDonosAnt { get; set; } = 0;
    }
}
