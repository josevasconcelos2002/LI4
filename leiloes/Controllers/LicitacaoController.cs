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
        public async Task<ActionResult<Licitacao>> Create([FromBody] Licitacao licitacao)
        {
            licitacao.Leilao = await _context.Leiloes.FindAsync(licitacao.leilao_IdLeilao);
            licitacao.Utilizador = await _context.Utilizadores.FindAsync(licitacao.user_Nif);

            var nif = licitacao.user_Nif;

            var utilizador = await _context.Utilizadores.FindAsync(nif);

            /*
            if (utilizador.Saldo < licitacao.Valor)
            {
                return BadRequest("Saldo insuficiente para a licitação.");
            }
            */

            utilizador.Saldo -= licitacao.Valor;
            _context.Update(utilizador);

            var leilao = await _context.Leiloes.FindAsync(licitacao.leilao_IdLeilao);
            leilao.LicitacaoAtual = licitacao.Valor;
            _context.Update(leilao);

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

