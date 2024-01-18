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
        public async Task<IActionResult> Create([Bind("Valor,IdLeilao,Nif,Data")] Licitacao licitacao)
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










        // APAGAR -> código de teste para criar uma licitacao
        public async Task<IActionResult> TestCreateLicitacao()
        {
            var licitacaoTeste = new Licitacao
            {
                // Preencha com dados de teste
                Valor = 1000m,
                leilao_IdLeilao = 1,
                user_Nif = "111111111"
            };

            _context.Licitacoes.Add(licitacaoTeste);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Ou retorne algum outro tipo de resposta
        }
    }






}

