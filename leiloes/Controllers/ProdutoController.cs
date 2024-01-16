using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models; 
using System.Threading.Tasks;

namespace leiloes.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly LeiloesDbContext _context;

        public ProdutoController(LeiloesDbContext context)
        {
            _context = context;
        }


        // Mostra a página dos produtos
        public IActionResult Index()
        {
            var produtos = _context.Produtos.ToList();
            return View(produtos);
        }

        
        
        // CREATE -> Mostra a página de criação do produto
        public IActionResult Create()
        {
            return View();
        }

        // CREATE -> Processa a criação do produto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Descricao,Imagem,NumDonosAnt")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Ajustar
            }

            return View(produto);
        }


        // DELETE -> Mostra a página de remoção do produto
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.IdProduto == id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        // DELETE -> Processa a remoção do produto
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Verifica se um produto existe -> Não usamos
        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.IdProduto == id);
        }
    }
}

