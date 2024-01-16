using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;

namespace leiloes.Controllers
{
    public class LeilaoController : Controller
    {
        private readonly LeiloesDbContext _context;

        public LeilaoController(LeiloesDbContext context)
        {
            _context = context;
        }


        // Mostra a página dos Leiloes
        public IActionResult Index()
        {
            var leiloes = _context.Leiloes
                .Include(l => l.Criador)  // Inclui a entidade Criador
                .Include(l => l.Produto)  // Inclui a entidade Produto
                .ToList();
            return View(leiloes);
        }



        // CREATE -> Mostra a página de criação do leiloes
        public IActionResult Create()
        {
            return View();
        }

        // CREATE -> Processa a criação do leilao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicitacaoAtual,PrecoMinLicitacao,Estado,DataInicial,DataFinal,CriadorId,ProdutoId")] Leilao leilao)
        {
            if (ModelState.IsValid)
            {
                _context.Leiloes.Add(leilao);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Ajustar
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro no campo {state.Key}: {error.ErrorMessage}");
                    }
                }
            }


            return View(leilao);
        }


        // DELETE -> Mostra a página de remoção do leilao
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var leilao = await _context.Leiloes
                .FirstOrDefaultAsync(m => m.IdLeilao == id);
            if (leilao == null)
            {
                return NotFound();
            }
            return View(leilao);
        }

        // DELETE -> Processa a remoção do leilao
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leilao = await _context.Leiloes.FindAsync(id);
            if (leilao != null)
            {
                _context.Leiloes.Remove(leilao);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Falta implementar: 
        // Aprovar leilão -> mudar o estado de pendente para ativo 

    }
}

