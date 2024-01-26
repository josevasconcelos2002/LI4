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
            var leilao = await _context.Leiloes.FindAsync(licitacao.leilao_IdLeilao);
            var user = await _context.Utilizadores.FindAsync(licitacao.user_Nif);
            licitacao.Leilao = leilao;
            licitacao.Utilizador = user;
            var nif = licitacao.user_Nif;
            var utilizador = await _context.Utilizadores.FindAsync(nif);

            // Utilizador não tem saldo
            if (utilizador.Saldo < licitacao.Valor)
            {
                return BadRequest("Saldo insuficiente para a licitação.");
            }

            // Licitação mais baixa/igual à última
            if (licitacao.Valor < leilao.LicitacaoAtual)
            {
                return BadRequest("Licitação demasiado baixa.");
            }

            // Licitação no próprio leilão
            if (leilao.CriadorId == user.Nif)
            {
                return BadRequest("Licitação inválida");
            }


            utilizador.Saldo -= licitacao.Valor;
            _context.Update(utilizador);

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

