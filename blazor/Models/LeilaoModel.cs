namespace blazor.Models
{
    public class LeilaoModel
    {
        public int IdLeilao { get; set; }
        public decimal LicitacaoAtual { get; set; }
        public decimal PrecoMinLicitacao { get; set; }
        public string Estado { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string CriadorId { get; set; }
        public int ProdutoId { get; set; }
    }
}
