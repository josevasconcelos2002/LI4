using System.Diagnostics;
using leiloes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace leiloes.Controllers
{
    [Route("api/[controller]")] // Define a rota para a API
    [ApiController] // Indica que este controller é uma API controller
    public class StatsController : ControllerBase
    {
        private readonly LeiloesDbContext _context;

        public StatsController(LeiloesDbContext context)
        {
            _context = context;
        }


        [HttpGet("{nif}")] 
        public async Task<ActionResult<LeilaoViewModel>> GetStats(string nif)
        {
            var ultimosDezLeiloes = _context.Leiloes
                .Where(l => l.CriadorId == nif && l.Estado == "terminado")
                .OrderByDescending(l => l.DataFinal)
                .Take(10)
                .Select(l => new { l.Produto.Nome, l.LicitacaoAtual });

            var dezMaioresVendas = _context.Leiloes
                .Where(l => l.CriadorId == nif)
                .OrderByDescending(l => l.LicitacaoAtual)
                .Take(10)
                .Select(l => new { l.Produto.Nome, l.LicitacaoAtual });

            var totalVendas = _context.Leiloes.Count(l => l.CriadorId == nif);
            var totalDinheiro = _context.Leiloes.Where(l => l.CriadorId == nif).Sum(l => l.LicitacaoAtual);

            var estatisticas = new LeilaoViewModel
            {
                UltimosDezLeiloes = await ultimosDezLeiloes.ToListAsync(),
                DezMaioresVendas = await dezMaioresVendas.ToListAsync(),
                TotalVendas = totalVendas,
                TotalDinheiro = totalDinheiro
            };

            return Ok(estatisticas);
        }

        public class LeilaoViewModel
        {
            public IEnumerable<object> UltimosDezLeiloes { get; set; }
            public IEnumerable<object> DezMaioresVendas { get; set; }
            public int TotalVendas { get; set; }
            public decimal TotalDinheiro { get; set; }
        }
    }
}
