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

        // Devolve todas as licitações
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Licitacao>>> GetLicitacoes()
        {
            return await _context.Licitacoes.ToListAsync();
        }

        // Devolve licitação por id
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

        // ---------------------- Criar Licitação ---------------------- 
        [HttpPost]
        public async Task<ActionResult<Licitacao>> Create([FromBody] Licitacao licitacao)
        {
            // Completar com os elementos que são definidos automaticamente
            var leilao = await _context.Leiloes.FindAsync(licitacao.leilao_IdLeilao);
            var user = await _context.Utilizadores.FindAsync(licitacao.user_Nif);
            licitacao.Leilao = leilao;
            licitacao.Utilizador = user;

            // Verificar se a licitação deve prosseguir
            var nif = licitacao.user_Nif;
            var utilizador = await _context.Utilizadores.FindAsync(nif);
            bool saldoInsuficiente = utilizador.Saldo < licitacao.Valor;
            bool licitacaoBaixa = licitacao.Valor < leilao.LicitacaoAtual || licitacao.Valor < leilao.PrecoMinLicitacao;
            bool proprioLeilao = leilao.CriadorId == user.Nif;
            bool leilaoNaoAtivo = leilao.Estado != "ativo";

            if (saldoInsuficiente || licitacaoBaixa || proprioLeilao || leilaoNaoAtivo)
            {
                return BadRequest("Licitação inválida.");
            }
            else
            {
                // Retirar o saldo correspondente à licitação
                utilizador.Saldo -= licitacao.Valor;
                _context.Update(utilizador);

                // Adicionar licitação ao leilão
                leilao.LicitacaoAtual = licitacao.Valor;
                _context.Update(leilao);

                // Adicionar a licitação à DB
                _context.Licitacoes.Add(licitacao);

                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetLicitacao), new { id = licitacao.IdLicitacao }, licitacao);
            }
        }
    }
}

