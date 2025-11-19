using Microsoft.EntityFrameworkCore;

namespace Comanda.Api
{
    public class ComandasDBContext : DbContext
    {
        public ComandasDBContext(
            DbContextOptions<ComandasDBContext> options
        ) : base(options)
        {   }
        // definir algumas configurracoes adicionais no banco
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Usuario>()
                .HasData(
                new Models.Usuario
                {
                    Id = 1,
                    Nome = "Admin",
                    Email = "admin@admin.com",
                    Senha = "admin123"
                }
            );

            modelBuilder.Entity<Models.CardapioItem>()
        
                .HasData(
                    new Models.CardapioItem
                    {
                        Id = 1,
                        Titulo = "Coxinha",
                        Descricao = "Coxinha de frango com catupiry",
                        Preco = 6.50m,
                        PossuiPreparo = true
                    },
                    new Models.CardapioItem
                    {
                        Id = 2,
                        Titulo = "Refrigerante",
                        Descricao = "Refrigerante lata 350ml",
                        Preco = 5.00m,
                        PossuiPreparo = false
                    },
                    new Models.CardapioItem
                    {
                        Id = 3,
                        Titulo = "Pizza Calabresa",
                        Descricao = "Pizza de calabresa com cebola",
                        Preco = 35.00m,
                        PossuiPreparo = true
                    }
                );
            modelBuilder.Entity<Models.Mesa>()
                .HasData(
                    new Models.Mesa
                    {
                        Id = 1,
                        NumeroMesa = 1,
                        SituacaoMesa = 0
                    },
                    new Models.Mesa
                    {
                        Id = 2,
                        NumeroMesa = 2,
                        SituacaoMesa = 1
                    },
                    new Models.Mesa
                    {
                        Id = 3,
                        NumeroMesa = 3,
                        SituacaoMesa = 2
                    }
                );
            // inserir as categorias no banco na primeira vez
            modelBuilder.Entity<Models.CategoriaCardapio>()
                .HasData(
                    new Models.CategoriaCardapio { Id = 1, Nome = "Lanches" },
                    new Models.CategoriaCardapio { Id = 2, Nome = "Bebidas" },
                    new Models.CategoriaCardapio { Id = 3, Nome = "Acompanhamentos" }
                );
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Models.Usuario> Usuarios { get; set; } = default!;
        public DbSet<Models.Mesa> Mesas { get; set; } = default!;
        public DbSet<Models.Reserva> Reservas { get; set; } = default!;
        public DbSet<Models.Comanda> Comandas { get; set; } = default!;
        public DbSet<Models.ComandaItem> ComandaItems { get; set; } = default!;
        public DbSet<Models.PedidoCozinha> PedidoCozinhas { get; set; } = default!;
        public DbSet<Models.PedidoCozinhaItem> PedidoCozinhaItems { get; set; } = default!;
        public DbSet<Models.CardapioItem> CardapioItems { get; set; } = default!;
        public DbSet<Models.CategoriaCardapio> CategoriaCardapio { get; set; } = default!;
    }
}
