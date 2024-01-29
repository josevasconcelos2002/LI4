namespace leiloes.Models
{
    public class StatsViewModel
    {
        public IEnumerable<object> UltimosDezLeiloes { get; set; }
        public IEnumerable<object> DezMaioresVendas { get; set; }
        public int TotalVendas { get; set; }
        public decimal TotalDinheiro { get; set; }
    }
}
