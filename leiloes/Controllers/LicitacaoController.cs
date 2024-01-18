﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;

namespace leiloes.Controllers
{
    public class LicitacaoController : Controller
    {
        private readonly LeiloesDbContext _context;

        public LicitacaoController(LeiloesDbContext context)
        {
            _context = context;
        }


        // Mostra a página das Licitações (isto nem sequer faz sentido e vai sair, mas pode ficar por agora só para testes)
        public IActionResult Index()
        {
            var licitacoes = _context.Licitacoes.ToList();
            return View(licitacoes);
        }

        // ---------------------- Criar Licitacao ----------------------
        // CREATE -> Mostra a página de criação de uma licitacao
        public IActionResult Create()
        {
            return View();
        }

        // CREATE -> Processa a criação de uma licitacao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Valor,IdLeilao,Data")] Licitacao licitacao)
        {
            // Extrair o NIF do utilizador do token JWT
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

            // Verificar se o saldo é suficiente
            if (utilizador.Saldo < licitacao.Valor)
            {
                return BadRequest("Saldo insuficiente para a licitação.");
            }

            // Atualizar o saldo do utilizador
            utilizador.Saldo -= licitacao.Valor;
            _context.Update(utilizador);

            licitacao.user_Nif = nif;

            // Adicionar a licitação
            _context.Licitacoes.Add(licitacao);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); 
        }
    
















        // ---------------------- TESTES (APAGAR) ----------------------
        // APAGAR -> código de teste para criar uma licitacao
        public async Task<IActionResult> TestCreateLicitacao()
        {
            var licitacaoTeste = new Licitacao
            {
                // Preencha com dados de teste
                Valor = 1000m,
                leilao_IdLeilao = 1,
                user_Nif = "111111111",
                dataLicitacao = DateTime.Now,
            };

            _context.Licitacoes.Add(licitacaoTeste);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Ou retorne algum outro tipo de resposta
        }
    }






}

