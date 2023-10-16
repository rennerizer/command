using Microsoft.EntityFrameworkCore;

namespace BoardGames.Api.Models
{
    public class BoardGamesContext : DbContext
    {
        public DbSet<BoardGame> BoardGames => Set<BoardGame>();

        public DbSet<Domain> Domains => Set<Domain>();

        public DbSet<Mechanic> Mechanics => Set<Mechanic>();

        public DbSet<BoardGameDomain> BoardGameDomains => Set<BoardGameDomain>();

        public DbSet<BoardGameMechanic> BoardGameMechanics => Set<BoardGameMechanic>();

        public BoardGamesContext() { }

        public BoardGamesContext(DbContextOptions<BoardGamesContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BoardGameDomain>()
                .HasKey(i => new { i.BoardGameId, i.DomainId });

            modelBuilder.Entity<BoardGameDomain>()
                .HasOne(x => x.BoardGame)
                .WithMany(y => y.BoardGameDomains)
                .HasForeignKey(f => f.BoardGameId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoardGameDomain>()
                .HasOne(o => o.Domain)
                .WithMany(m => m.BoardGameDomains)
                .HasForeignKey(f => f.DomainId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoardGameMechanic>()
                .HasKey(i => new { i.BoardGameId, i.MechanicId });

            modelBuilder.Entity<BoardGameMechanic>()
                .HasOne(x => x.BoardGame)
                .WithMany(y => y.BoardGameMechanics)
                .HasForeignKey(f => f.BoardGameId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoardGameMechanic>()
                .HasOne(o => o.Mechanic)
                .WithMany(m => m.BoardGameMechanics)
                .HasForeignKey(f => f.MechanicId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
