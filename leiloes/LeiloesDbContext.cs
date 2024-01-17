using Microsoft.EntityFrameworkCore;
using leiloes.Models; 

namespace leiloes
{
    public class LeiloesDbContext : DbContext
    {
        public LeiloesDbContext(DbContextOptions<LeiloesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Leilao> Leiloes { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Licitacao> Licitacoes { get; set; }
    }



}
