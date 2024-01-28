using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace blazor.Models
{
    public class ProdutoModel
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Imagem { get; set; }
        public int NumDonosAnt { get; set; } = 0;
    }
}
