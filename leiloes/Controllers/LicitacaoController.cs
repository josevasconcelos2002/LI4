using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;

namespace leiloes.Controllers
{
    public class LicitacaoController : Controller
    {
        private readonly LeiloesDbContext _context;

        public LicitacaoController(LeiloesDbContext context)
        {
            _context = context;
        }


        // Mostra a página das Licitações (isto nem sequer faz sentido e vai sair, mas pode ficar por agora só para testes)
        public IActionResult Index()
        {
            var licitacoes = _context.Licitacoes.ToList();
            return View(licitacoes);
        }


        // CREATE -> Mostra a página de criação de uma licitacao
        public IActionResult Create()
        {
            return View();
        }

        // CREATE -> Processa a criação de uma licitacao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Valor,IdLeilao,Nif")] Licitacao licitacao)
        {
            if (ModelState.IsValid)
            {
                _context.Licitacoes.Add(licitacao);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Ajustar
            }

            return View(licitacao);
        }
    
        // Falta implementar: 
        // Penso que nada. A única cena necessária aqui é criar a licitação

    }
}

