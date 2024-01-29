using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace leiloes.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class LeilaoController : ControllerBase
    {
        private readonly LeiloesDbContext _context;

        public LeilaoController(LeiloesDbContext context)
        {
            _context = context;
        }

        // Devolve todos os leilões
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leilao>>> GetLeiloes()
        {
            var leiloes = await _context.Leiloes.ToListAsync();
            return Ok(leiloes);
        }

        // Devolve o leilão por id
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
            // Verificar se o valor de abertura é maior do que 0 e se a data final não é no passado
            bool precoMinValido = leilao.PrecoMinLicitacao >= 0;
            bool dataFinalValida = leilao.DataFinal >= DateTime.Now;
            
            if (!precoMinValido || !dataFinalValida)
            {
                return BadRequest("Valor(es) inválido(s)");
            }

            // Completar com os elementos que são definidos automaticamente
            leilao.DataInicial = DateTime.Now;
            leilao.LicitacaoAtual = 0.00M;
            leilao.Estado = "pendente";
            leilao.Produto = await _context.Produtos.FindAsync(leilao.ProdutoId);
            leilao.Criador = await _context.Utilizadores.FindAsync(leilao.CriadorId);

            _context.Leiloes.Add(leilao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeilao), new { id = leilao.IdLeilao }, leilao);
        }


        // ---------------------- Eliminar Leilao ----------------------
        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete(int id)
        {
            var leilao = await _context.Leiloes.FindAsync(id);
            if (leilao == null)
            {
                return NotFound();
            }

            int produtoId = leilao.ProdutoId;

            _context.Leiloes.Remove(leilao);
            await _context.SaveChangesAsync();

            // Devolve o ID do produto associado ao leilão deletado
            // para podermos eliminar o produto 
            return produtoId;
        }


        // ---------------------- Aprovar Leilao ----------------------
        [HttpPost("aprovarLeilao/{leilaoId}")]
        public async Task<IActionResult> AprovarLeilao(int leilaoId)
        {
            var leilao = await _context.Leiloes.FindAsync(leilaoId);

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


        // ---------------------- Consultar Leiloes por User ---------------------- 
        [HttpGet("leiloesUser/{nif}")]
        public async Task<ActionResult<IEnumerable<Leilao>>> LeiloesUser(string nif)
        {
            var leiloes = await _context.Leiloes
                .Where(l => l.CriadorId == nif) 
                .Select(l => new LeilaoViewModel
                {
                    LeilaoId = l.IdLeilao,
                    NomeItem = l.Produto.Nome,
                    ValorAtualLicitacao = l.LicitacaoAtual

                })
                .ToListAsync();

            return Ok(leiloes);
        }
    }
}


