using Microsoft.AspNetCore.Mvc;
using leiloes.Data;
using System.Linq;

namespace leiloes.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly LeiloesDbContext _context;

        public ProdutosController(LeiloesDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var produtos = _context.Produtos.ToList();
            return View(produtos);
        }
    }
}
