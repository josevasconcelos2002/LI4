using Microsoft.EntityFrameworkCore;
using leiloes.Data; // isto corrige um erro for some reason

namespace leiloes.Data
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
