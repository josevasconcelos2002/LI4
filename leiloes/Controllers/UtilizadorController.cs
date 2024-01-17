using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;



namespace leiloes.Controllers
{
    public class UtilizadorController : Controller
    {
        private readonly LeiloesDbContext _context;
        private readonly IConfiguration _configuration;

        public UtilizadorController(LeiloesDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }




        // ---------------------- Criar Utilizador (Registo) ----------------------
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nif,Nome,Username,Email,Password")] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(utilizador.Password));
                    var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower().Substring(0, 45); // Fazemos isto porque usamos VARCHAR(45)
                    utilizador.Password = hash;
                }

                utilizador.UserType = 0;
                utilizador.Saldo = 0.00M;

                _context.Utilizadores.Add(utilizador);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(utilizador);
        }



        // ---------------------- Eliminar Utilizador (Registo) ----------------------

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




        // ---------------------- Autenticar Utilizador ----------------------
        private string GenerateJwtToken(Utilizador user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // Outros claims conforme necessário
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Hash da senha fornecida
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower().Substring(0, 45);

                var user = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.Username == username && u.Password == hashedPassword);

                if (user != null)
                {
                    var tokenString = GenerateJwtToken(user);
                    return Ok(new { Token = tokenString });
                }
            }

            return Unauthorized();
        }



        // ---------------------- Desconectar Utilizador ----------------------
        // Aparentemente não é preciso nada aqui



        // ---------------------- Consultar Perfil ----------------------
        [Authorize]
        [HttpGet("perfil/{userId}")]
        public async Task<IActionResult> Perfil() 
        {
            // Extrair o username do utilizador do token JWT
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            // Procurar o utilizador pelo username para obter o NIF (isto seria muito mais simples se o username fosse a chave primária)
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.Username == username);

            if (utilizador == null)
            {
                return NotFound();
            }

            var nif = utilizador.Nif;

            // Busca os leilões criados pelo usuário
            var leiloesCriados = await _context.Leiloes
                .Where(l => l.CriadorId == nif)
                .ToListAsync();

            // Dividir os leilões em ativos e não ativos
            var leiloesAtivos = leiloesCriados.Where(l => l.Estado == "Ativo").ToList();
            var leiloesNaoAtivos = leiloesCriados.Where(l => l.Estado != "Ativo").ToList();

            // Construir o objeto de perfil
            var perfil = new
            {
                NomeCompleto = utilizador.Nome,
                Username = utilizador.Username,
                Email = utilizador.Email,
                Nif = utilizador.Nif,
                LeiloesAtivos = leiloesAtivos,
                LeiloesNaoAtivos = leiloesNaoAtivos
            };

            return Ok(perfil);
        }

















        // ---------------------- TESTES (APAGAR) ----------------------
        // APAGAR -> código de teste para verificar login
        public async Task<IActionResult> TestLogin()
        {
            // Dados do usuário para teste
            var testeUsername = "luisborges";
            var testePassword = "pass123";

            // Simula a lógica de verificação de usuário e senha
            var user = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.Username == testeUsername && u.Password == testePassword); 

            if (user != null)
            {
                // Aqui você chamaria o método para gerar o token JWT, como no método de login real
                var tokenString = GenerateJwtToken(user);

                // Retorna o token gerado ou uma resposta de sucesso
                return Ok(new { Token = tokenString });
            }

            // Caso o login falhe
            return Unauthorized();
        }
















     

    }

}

