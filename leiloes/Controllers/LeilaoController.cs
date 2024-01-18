using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace leiloes.Controllers
{
    public class LeilaoController : Controller
    {
        private readonly LeiloesDbContext _context;

        public LeilaoController(LeiloesDbContext context)
        {
            _context = context;
        }

        // ---------------------- Criar Leilao ----------------------
        // CREATE -> Mostra a página de criação do leiloes
        public IActionResult Index()
        {
            var leiloes = _context.Leiloes.ToList();
            return View(leiloes);
        }

        // CREATE -> Processa a criação do leilao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicitacaoAtual,PrecoMinLicitacao,Estado,DataInicial,DataFinal,CriadorId,ProdutoId")] Leilao leilao)
        {

            Debug.WriteLine($"CriadorId: {leilao.CriadorId}, ProdutoId: {leilao.ProdutoId}");


            if (ModelState.IsValid)
            {

                leilao.LicitacaoAtual = 0.00M; // um leilão começa sem licitações
                leilao.Estado = "pendente"; // o leilão fica em estado pendente até ser aprovado por um administrador

                _context.Leiloes.Add(leilao);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Ajustar
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro no campo {state.Key}: {error.ErrorMessage}");
                    }
                }
            }

            return View(leilao);
        }



        // ---------------------- Eliminar Leilao ----------------------
        // DELETE -> Mostra a página de remoção do leilao
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var leilao = await _context.Leiloes
                .FirstOrDefaultAsync(m => m.IdLeilao == id);
            if (leilao == null)
            {
                return NotFound();
            }
            return View(leilao);
        }

        // DELETE -> Processa a remoção do leilao
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leilao = await _context.Leiloes.FindAsync(id);
            if (leilao != null)
            {
                _context.Leiloes.Remove(leilao);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
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



        // ---------------------- Aprovar Leilao ----------------------
        [HttpGet("consultarLeilao/{leilaoId}")]
        public async Task<IActionResult> ConsultarLeilao(int leilaoId)
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





























        // ---------------------- TESTES (APAGAR) ----------------------
        // APAGAR -> código de teste para criar um leilão
        public async Task<IActionResult> TestCreateLeilao()
        {
            var leilaoTeste = new Leilao
            {
                // Preencha com dados de teste
                LicitacaoAtual = 1000m,
                PrecoMinLicitacao = 500m,
                Estado = "Ativo",
                DataInicial = DateTime.Now,
                DataFinal = DateTime.Now.AddDays(7),
                CriadorId = "333333333",  // Certifique-se de que este ID exista no seu banco de dados
                ProdutoId = 1     // Certifique-se de que este ID exista no seu banco de dados
            };

            _context.Leiloes.Add(leilaoTeste);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Ou retorne algum outro tipo de resposta
        }

    }

}

