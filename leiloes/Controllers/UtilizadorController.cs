using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;

namespace leiloes.Controllers
{
    public class UtilizadorController : Controller
    {
        private readonly LeiloesDbContext _context;

        public UtilizadorController(LeiloesDbContext context)
        {
            _context = context;
        }


        // Mostra a página dos utilizadores
        public IActionResult Index()
        {
            var utilizadores = _context.Utilizadores.ToList();
            return View(utilizadores);
        }

        // CREATE -> Mostra a página de criação do utilizador
        public IActionResult Create()
        {
            return View();
        }

        // CREATE -> Processa a criação do utilizador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nif,Nome,Username,Email,Password,UserType,Saldo")] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                _context.Utilizadores.Add(utilizador);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Ajustar
            }

            return View(utilizador);
        }


        // DELETE -> Mostra a página de remoção do utilizador
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (utilizador == null)
            {
                return NotFound();
            }
            return View(utilizador);
        }

        // DELETE -> Processa a remoção do utilizador
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        { 
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador != null)
            {
                _context.Utilizadores.Remove(utilizador);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}

