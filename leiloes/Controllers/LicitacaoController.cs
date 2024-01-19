using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;
using Microsoft.AspNetCore.Authorization;

namespace leiloes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicitacaoController : ControllerBase
    {
        private readonly LeiloesDbContext _context;

        public LicitacaoController(LeiloesDbContext context)
        {
            _context = context;
        }

        // GET: api/Licitacao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Licitacao>>> GetLicitacoes()
        {
            return await _context.Licitacoes.ToListAsync();
        }

        // POST: api/Licitacao
        [HttpPost]
        [Authorize] // Garante que apenas usuários autenticados possam criar uma licitação
        public async Task<ActionResult<Licitacao>> Create([FromBody] Licitacao licitacao)
        {
            var nif = User.FindFirst("Nif")?.Value;

            if (string.IsNullOrEmpty(nif))
            {
                return Unauthorized();
            }

            var utilizador = await _context.Utilizadores.FindAsync(nif);
            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            if (utilizador.Saldo < licitacao.Valor)
            {
                return BadRequest("Saldo insuficiente para a licitação.");
            }

            utilizador.Saldo -= licitacao.Valor;
            _context.Update(utilizador);

            licitacao.user_Nif = nif;

            _context.Licitacoes.Add(licitacao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLicitacao), new { id = licitacao.IdLicitacao }, licitacao);
        }

        // GET: api/Licitacao/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Licitacao>> GetLicitacao(int id)
        {
            var licitacao = await _context.Licitacoes.FindAsync(id);
            if (licitacao == null)
            {
                return NotFound();
            }
            return licitacao;
        }
    }
}

