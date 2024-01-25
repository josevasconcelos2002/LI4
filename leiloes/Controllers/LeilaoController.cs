using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace leiloes.Controllers
{
    [Route("api/[controller]")] // Define a rota para a API
    [ApiController]
    public class LeilaoController : ControllerBase
    {
        private readonly LeiloesDbContext _context;

        public LeilaoController(LeiloesDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leilao>>> GetLeiloes()
        {
            var leiloes = await _context.Leiloes.ToListAsync();
            return Ok(leiloes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Leilao>> GetLeilao(int id)
        {
            var leilao = await _context.Leiloes.FindAsync(id);

            if (leilao == null)
            {
                return NotFound();
            }

            return leilao;
        }


     



        // ---------------------- Criar Leilao ----------------------
        [HttpPost]
        public async Task<ActionResult<Leilao>> Create([FromBody] Leilao leilao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            leilao.DataInicial = DateTime.Now;
            leilao.LicitacaoAtual = 0.00M;
            leilao.Estado = "pendente";

            _context.Leiloes.Add(leilao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeilao), new { id = leilao.IdLeilao }, leilao);
        }



        // ---------------------- Eliminar Leilao ----------------------
        // DELETE -> Mostra a página de remoção do leilao
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var leilao = await _context.Leiloes.FindAsync(id);
            if (leilao == null)
            {
                return NotFound();
            }

            _context.Leiloes.Remove(leilao);
            await _context.SaveChangesAsync();

            return NoContent();
        }




        // ---------------------- Aprovar Leilao ----------------------
        [Authorize]
        [HttpPost("aprovarLeilao/{leilaoId}")]
        public async Task<IActionResult> AprovarLeilao(int leilaoId)
        {
            // Extrair o UserType do token JWT
            var userType = User.FindFirst("UserType")?.Value;

            if (userType != "1")
            {
                return Unauthorized("Acesso negado: usuário não é administrador.");
            }

            var leilao = await _context.Leiloes
                .FirstOrDefaultAsync(l => l.IdLeilao == leilaoId);

            if (leilao == null)
            {
                return NotFound();
            }

            if (leilao.Estado != "pendente")
            {
                return BadRequest("O leilão já está ativo ou já terminou.");
            }

            // Aprovar o leilão
            leilao.Estado = "ativo";
            _context.Update(leilao);
            await _context.SaveChangesAsync();

            return Ok("Leilão aprovado com sucesso.");
        }



        // ---------------------- Consultar Leilao ---------------------- (isto é o mesmo que o get)
        [HttpGet("consultarLeilao/{leilaoId}")]
        public async Task<ActionResult> ConsultarLeilao(int leilaoId)
        {
            var leilao = await _context.Leiloes
                .Include(l => l.Produto) 
                .FirstOrDefaultAsync(l => l.IdLeilao == leilaoId);

            if (leilao == null)
            {
                return NotFound();
            }

            // Construir o objeto de resposta
            var resposta = new
            {
                NomeItem = leilao.Produto.Nome,
                DescricaoItem = leilao.Produto.Descricao,
                ImagemItem = leilao.Produto.Imagem,
                NumDonosAnteriores = leilao.Produto.NumDonosAnt,
                ValorInicialLicitacao = leilao.PrecoMinLicitacao,
                LicitacaoAtual = leilao.LicitacaoAtual,
                DataInicio = leilao.DataInicial,
                DataFim = leilao.DataFinal
            };

            return Ok(resposta);
        }

    }

}

