using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using leiloes.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;




namespace leiloes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UtilizadorController : ControllerBase
    {
        private readonly LeiloesDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UtilizadorController> _logger;


        public UtilizadorController(LeiloesDbContext context, IConfiguration configuration, ILogger<UtilizadorController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }


        // ---------------------- Lista de utilizadores ----------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilizador>>> GetUtilizadores()
        {
            return await _context.Utilizadores.ToListAsync();
        }


        // ---------------------- Utilizador através do nif ----------------------
        [HttpGet("{nif}")]
        public async Task<ActionResult<Utilizador>> GetUtilizador(string nif)
        {
            var utilizador = await _context.Utilizadores.FindAsync(nif);
            if (utilizador == null)
            {
                return NotFound();
            }
            return utilizador;
        }


        // ---------------------- Criar Utilizador (Registo) ----------------------
        [HttpPost("create")]
        public async Task<ActionResult<Utilizador>> Create([FromBody] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                // Verificar se Nif, Username e Email são únicos
                bool nifExists = await _context.Utilizadores.AnyAsync(u => u.Nif == utilizador.Nif);
                bool usernameExists = await _context.Utilizadores.AnyAsync(u => u.Username == utilizador.Username);
                bool emailExists = await _context.Utilizadores.AnyAsync(u => u.Email == utilizador.Email);

                if (nifExists || usernameExists || emailExists)
                {
                    return BadRequest("Nif, Username ou Email já existem na base de dados.");
                }

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
                var tokenString = GenerateJwtToken(utilizador);
                return Ok(new { Token = tokenString });
            }

            return NoContent();
        }


        // ---------------------- Eliminar Utilizador ----------------------
        [HttpDelete("{nif}")]
        public async Task<IActionResult> Delete(string nif)
        {
            {
                var utilizador = await _context.Utilizadores.FindAsync(nif);
                if (utilizador != null)
                {
                    _context.Utilizadores.Remove(utilizador);
                    await _context.SaveChangesAsync();
                }
                return NoContent();
            }
        }


        // ---------------------- Autenticar Utilizador ----------------------
        private string GenerateJwtToken(Utilizador user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserType", user.UserType.ToString()),
                new Claim("Nif", user.Nif) 
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
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation("oláaa");
            var user = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user != null)
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(loginRequest.Password));
                    var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower().Substring(0, 45);

                    if (user.Password == hashedPassword)
                    {
                        var tokenString = GenerateJwtToken(user);
                        return Ok(new { Token = tokenString });
                    }
                }
            }

            return Unauthorized("Credenciais inválidas.");
        }


        // ---------------------- Consultar Perfil ----------------------
        [Authorize]
        [HttpGet("perfil/{userId}")]
        public async Task<IActionResult> Perfil()
        {
            // Extrair o NIF do utilizador do token JWT
            var nif = User.FindFirst("Nif")?.Value; 

            if (string.IsNullOrEmpty(nif))
            {
                return Unauthorized();
            }

            // Procurar o utilizador pelo NIF
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.Nif == nif);

            if (utilizador == null)
            {
                return NotFound();
            }

            var leiloesCriados = await _context.Leiloes
                .Where(l => l.CriadorId == nif)
                .ToListAsync();

            var leiloesAtivos = leiloesCriados.Where(l => l.Estado == "Ativo").ToList();
            var leiloesNaoAtivos = leiloesCriados.Where(l => l.Estado != "Ativo").ToList();

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


        // ---------------------- Adicionar Saldo ----------------------
        [HttpPost("adicionarSaldo")]
        public async Task<IActionResult> AdicionarSaldo([FromBody] SaldoRequest request)
        {
            var utilizador = await _context.Utilizadores.FindAsync(request.Nif);
            if (utilizador == null)
            {
                return NotFound();
            }

            if(request.Amount < 0)
            {
                return BadRequest("Não é possível adicionar saldo negativo");
            }

            utilizador.Saldo += request.Amount;
            _context.Update(utilizador);
            await _context.SaveChangesAsync();

            return Ok("Saldo adicionado com sucesso.");
        }


        // ---------------------- Classe usada no login ----------------------
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        // ---------------------- Classe usada na adição de saldo ----------------------
        public class SaldoRequest
        {
            public string Nif { get; set; }
            public decimal Amount { get; set; }
        }



    }

}

