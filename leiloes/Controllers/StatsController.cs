using System.Diagnostics;
using leiloes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace leiloes.Controllers
{
    [Route("api/[controller]")] 
    [ApiController] 
    public class StatsController : ControllerBase
    {
        private readonly LeiloesDbContext _context;

        public StatsController(LeiloesDbContext context)
        {
            _context = context;
        }

        // Devolve estatísticas por user
        [HttpGet("{nif}")] 
        public async Task<ActionResult<StatsViewModel>> GetStats(string nif)
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

            var estatisticas = new StatsViewModel
            {
                UltimosDezLeiloes = await ultimosDezLeiloes.ToListAsync(),
                DezMaioresVendas = await dezMaioresVendas.ToListAsync(),
                TotalVendas = totalVendas,
                TotalDinheiro = totalDinheiro
            };

            return Ok(estatisticas);
        }
    }
}
