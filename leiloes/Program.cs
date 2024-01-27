using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using leiloes;
using System.Threading;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Adicionar servi�os ao container.
builder.Services.AddDbContext<leiloes.LeiloesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Configura��o do JWT
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

// Configurar o timer para verificar os leil�es
var timer = new Timer(CheckLeiloes, null, 0, 60000); // Executa a cada minutos


// Configure o pipeline de requisi��es HTTP.
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
    // Obt�m uma inst�ncia do escopo de servi�o
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
        logger.LogError(ex, "Erro ao verificar e terminar leil�es");
    }
}

void VerificarETerminarLeiloes(LeiloesDbContext dbContext)
{
    // Obter todos os leil�es ativos que passaram da DataFinal
    var leiloesParaVerificar = dbContext.Leiloes
        .Where(l => l.Estado == "ativo" && l.DataFinal <= DateTime.Now)
        .ToList();

    foreach (var leilao in leiloesParaVerificar)
    {
        // Verificar se houve alguma licita��o nos �ltimos 300 segundos
        var inicioDoIntervalo = DateTime.Now.AddMinutes(-5);
        if (!dbContext.Licitacoes.Any(lic => lic.leilao_IdLeilao == leilao.IdLeilao && lic.dataLicitacao >= inicioDoIntervalo))
        {
            // Se n�o houve licita��es, atualizar o estado do leil�o para "terminado"
            leilao.Estado = "terminado";
            dbContext.Update(leilao);
        }
    }

    dbContext.SaveChanges();
}



