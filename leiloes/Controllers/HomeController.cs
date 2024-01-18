using System.Diagnostics;
using leiloes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace leiloes.Controllers
{
    public class HomeController : Controller
    {
        private readonly LeiloesDbContext _context;

        public HomeController(LeiloesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var leiloesAtivos = await _context.Leiloes
                .Where(l => l.Estado == "ativo") // Assumindo que "ativo" é um estado do leilão
                .Select(l => new LeilaoViewModel
                {
                    LeilaoId = l.IdLeilao,
                    NomeItem = l.Produto.Nome, // Assumindo relação entre Leilao e Produto
                    ValorAtualLicitacao = l.LicitacaoAtual
                })
                .ToListAsync();

            return View(leiloesAtivos);
        }
    }

}
