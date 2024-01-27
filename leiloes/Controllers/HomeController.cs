using System.Diagnostics;
using leiloes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace leiloes.Controllers
{
    [Route("api/[controller]")] // Define a rota para a API
    [ApiController] // Indica que este controller é uma API controller
    public class HomeController : ControllerBase
    {
        private readonly LeiloesDbContext _context;

        public HomeController(LeiloesDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeilaoViewModel>>> Index()
        {
            var leiloesAtivos = await _context.Leiloes
                .Where(l => l.Estado == "ativo")
                .Select(l => new LeilaoViewModel
                {
                    LeilaoId = l.IdLeilao,
                    NomeItem = l.Produto.Nome,
                    ValorAtualLicitacao = l.LicitacaoAtual
                })
                .ToListAsync();

            return Ok(leiloesAtivos);
        }

        [HttpGet("pendentes")]
        public async Task<ActionResult<IEnumerable<LeilaoViewModel>>> GetLeiloesPendentes()
        {
            var leiloesPendentes = await _context.Leiloes
                .Where(l => l.Estado == "pendente")
                .Select(l => new LeilaoViewModel
                {
                    LeilaoId = l.IdLeilao,
                    NomeItem = l.Produto.Nome,
                    ValorAtualLicitacao = l.LicitacaoAtual
                })
                .ToListAsync();
            
            return Ok(leiloesPendentes);
        }
    }
}
