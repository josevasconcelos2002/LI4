﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace leiloes.Controllers
{
    [Route("api/[controller]")] // Define a rota para a API
    [ApiController]
    public class LeilaoController : ControllerBase
    {
        private readonly LeiloesDbContext _context;
        private readonly ILogger<LeilaoController> _logger;

        public LeilaoController(LeiloesDbContext context, ILogger<LeilaoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leilao>>> GetLeiloes()
        {
            _logger.LogInformation("olá1");
            var leiloes = await _context.Leiloes.ToListAsync();
            return Ok(leiloes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Leilao>> GetLeilao(int id)
        {
            _logger.LogInformation("olá2");
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
            var produto = await _context.Produtos.FindAsync(leilao.ProdutoId);
            var criador = await _context.Utilizadores.FindAsync(leilao.CriadorId);

            leilao.DataInicial = DateTime.Now;
            leilao.LicitacaoAtual = 0.00M;
            leilao.Estado = "pendente";
            leilao.Produto = produto;
            leilao.Criador = criador;


            _context.Leiloes.Add(leilao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeilao), new { id = leilao.IdLeilao }, leilao);
        }



        // ---------------------- Eliminar Leilao ----------------------
        // DELETE -> Mostra a página de remoção do leilao
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("olá4");
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
            _logger.LogInformation("olá5");
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



        // ---------------------- Consultar LeiloesUser ---------------------- 
        [HttpGet("leiloesUser/{nif}")]
        public async Task<ActionResult<IEnumerable<Leilao>>> LeiloesUser(string nif)
        {
            _logger.LogInformation("olá6");
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


