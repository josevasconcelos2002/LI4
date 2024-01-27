using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using leiloes;
using System.Threading;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container.
builder.Services.AddDbContext<leiloes.LeiloesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Configuração do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


var app = builder.Build();

// Configurar o timer para verificar os leilões
var timer = new Timer(CheckLeiloes, null, 0, 60000); // Executa a cada minutos


// Configure o pipeline de requisições HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

app.Run();

void CheckLeiloes(object state)
{
    // Obtém uma instância do escopo de serviço
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<leiloes.LeiloesDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
      
        VerificarETerminarLeiloes(dbContext);

    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao verificar e terminar leilões");
    }
}

void VerificarETerminarLeiloes(LeiloesDbContext dbContext)
{
    // Obter todos os leilões ativos que passaram da DataFinal
    var leiloesParaVerificar = dbContext.Leiloes
        .Where(l => l.Estado == "ativo" && l.DataFinal <= DateTime.Now)
        .ToList();

    foreach (var leilao in leiloesParaVerificar)
    {
        // Verificar se houve alguma licitação nos últimos 300 segundos
        var inicioDoIntervalo = DateTime.Now.AddMinutes(-5);
        if (!dbContext.Licitacoes.Any(lic => lic.leilao_IdLeilao == leilao.IdLeilao && lic.dataLicitacao >= inicioDoIntervalo))
        {
            // Se não houve licitações, atualizar o estado do leilão para "terminado"
            leilao.Estado = "terminado";
            dbContext.Update(leilao);
        }
    }

    dbContext.SaveChanges();
}



